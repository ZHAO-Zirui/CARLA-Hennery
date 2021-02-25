using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using CARLA_Hennery.Master.Managers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CARLA_Hennery.Master.Services
{
    public class CarlaVersionCaptureService
    {
        private readonly ILogger<CarlaVersionCaptureService> _logger;
        private readonly IConfiguration _configuration;
        private readonly VersionManager _version;

        private Thread _thread;
        private bool _isConfigurationOk;

        public string Repositories { get; set; }
        public int NextSleepMillisecond { get; set; }
        public int FailureSleepMillisecond { get; set; } = 3600000; // After 1 hour.

        public CarlaVersionCaptureService(ILogger<CarlaVersionCaptureService> logger,
            IConfiguration configuration, VersionManager version)
        {
            _logger = logger;
            _configuration = configuration;
            _version = version;
        }

        public void Run()
        {
            try
            {
                Repositories = _configuration["CARLA:Repositories"];
                NextSleepMillisecond = int.Parse(_configuration["VersionCapture:SleepMillisecond"]);

                // All set seems good
                _isConfigurationOk = true;
            }
            catch (Exception e)
            {
                _logger.LogCritical("Loss CARLA configuration in file:'appsettings.json'\n{E}",e);
                _isConfigurationOk = false;
            }
            
            if (!_isConfigurationOk)
            {
                _logger.LogCritical("Configuration failure. Capture thread will not start");
                return;
            }

            _thread = new Thread(Capture);
            _thread.Start();
        }

        private void Capture()
        {
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = "git",
                    Arguments = $"ls-remote --tags {Repositories}",
                    RedirectStandardOutput = true
                }
            };

            while (true)
            {
                try
                {
                    _logger.LogInformation("Start CARLA version capture from {Repositories}",Repositories);
                    process.Start();
                    var result = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();
                    _logger.LogInformation("CARLA version capture complete, start comparison");
                    
                    var lines = result.Split('\n');
                    var dropCount = 0;
                    var addCount = 0;

                    foreach (var line in lines)
                    {
                        // drop useless parts
                        if (line.EndsWith("^{}"))
                        {
                            dropCount++;
                            continue;
                        }

                        var parts = line.Split('\t');
                        
                        // drop failure parts
                        if (parts.Length != 2)
                        {
                            dropCount++;
                            continue;
                        }

                        var tag = parts[1];
                        var versionStr = tag.Split('/').Last();
                        var versionParts = versionStr.Split('.');
                        var version = new Library.CARLA.Version(
                            int.Parse(versionParts[0]),
                            int.Parse(versionParts[1]),
                            int.Parse(versionParts[2]));

                        // Tag have additional parts
                        if (versionParts.Length > 3)
                        {
                            version.Additional = int.Parse(versionParts[3]);
                        }
                        
                        // Check is this version in database
                        if (!_version.IsCarlaVersionKnown(version))
                        {
                            // If not, add it.
                            _version.AddCarlaVersion(version);
                            addCount++;
                        }
                    }
                    
                    // Exec report
                    _logger.LogInformation("CARLA version synchronized with git remote complete\n" +
                                           "Found:{Length}\n" +
                                           "Drop:{Drop}\n" +
                                           "Add:{Add}"
                        ,lines.Length,dropCount,addCount);
                    _logger.LogInformation("CARLA version will sleep {Sleep} ms." +
                                           "Next execute on {Exec}",NextSleepMillisecond,
                        DateTime.Now.AddMilliseconds(NextSleepMillisecond).ToString("s"));
                    Thread.Sleep(NextSleepMillisecond);
                }
                catch (Exception e)
                {
                    _logger.LogError("Capture process failure:\n{E}",e);
                    
                    _logger.LogError("Capture thread will sleep {FailureSleepMillisecond}ms. " +
                                     "Next execute on {Exec}",
                        FailureSleepMillisecond,DateTime.Now.AddMilliseconds(FailureSleepMillisecond).ToString("s"));
                    Thread.Sleep(FailureSleepMillisecond);
                    continue;
                }
            }
            
        }
        
    }
}