using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using CARLA_Hennery.Master.Managers;
using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CARLA_Hennery.Master.Services
{
    public class PythonVersionCaptureService
    {
        private readonly ILogger<PythonVersionCaptureService> _logger;
        private readonly IConfiguration _configuration;
        private readonly VersionManager _version;

        private Thread _thread;
        private bool _isConfigurationOk;

        public string Target { get; set; }
        public int NextSleepMillisecond { get; set; }
        public int FailureSleepMillisecond { get; set; } = 3600000; // After 1 hour.

        public PythonVersionCaptureService(ILogger<PythonVersionCaptureService> logger,
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
                Target = _configuration["Python:Target"];
                NextSleepMillisecond = int.Parse(_configuration["VersionCapture:SleepMillisecond"]);

                // All set seems good
                _isConfigurationOk = true;
            }
            catch (Exception e)
            {
                _logger.LogCritical("Loss Python configuration in file:'appsettings.json'\n{E}",e);
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
                    FileName = "curl",
                    Arguments = $"-s {Target}",
                    RedirectStandardOutput = true
                }
            };

            while (true)
            {
                try
                {
                    // Download HTML
                    _logger.LogInformation("Start python version capture from {Repositories}",Target);
                    process.Start();
                    var result = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();
                    _logger.LogInformation("Python version capture complete, start comparison");
                    
                    // Comparison
                    var doc = new HtmlDocument();
                    doc.LoadHtml(result);
                    var nodes = doc.DocumentNode.SelectNodes("//pre/a");

                    var dropCount = 0;
                    var addCount = 0;

                    foreach (var node in nodes)
                    {
                        // Get python version str
                        var line = node.Attributes["href"].Value;
                        // drop rc version
                        if (line.Contains("rc") || line.Contains("p"))
                        {
                            dropCount++;
                            continue;
                        }

                        var versionStr = line.Replace("/",null);
                        var versionParts = versionStr.Split('.');

                        if (versionParts[0] == "" || versionParts[1] == "")
                        {
                            dropCount++;
                            continue;
                        }else if (versionParts.Length == 3 && versionParts[2] == "")
                        {
                            dropCount++;
                            continue;
                        }


                        var additional = versionParts.Length == 2 ? 0 : int.Parse(versionParts[2]);
                        var version = new Library.Python.Version(
                            int.Parse(versionParts[0]),
                            int.Parse(versionParts[1]),
                            additional);
                        
                        // Check is this version in database
                        if (!_version.IsPythonVersionKnown(version))
                        {
                            // If not, add it.
                            _version.AddPythonVersion(version);
                            addCount++;
                        }
                    }
                    
                    // Exec report
                    _logger.LogInformation("Python version synchronized with git remote complete\n" +
                                           "Found:{Length}\n" +
                                           "Drop:{Drop}\n" +
                                           "Add:{Add}"
                        ,nodes.Count,dropCount,addCount);
                    _logger.LogInformation("Python version will sleep {Sleep} ms." +
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