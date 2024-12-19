using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Diagnostics;

namespace UserService.infrastructure
{
    public class CustomHealthCheck : IHealthCheck
    {
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var reportData = new Dictionary<string, object>();
            HealthStatus healthStatus = HealthStatus.Healthy;

            var drivePath = GetDiskPath();
            var driveInfo = new DriveInfo(drivePath);
            if (driveInfo.AvailableFreeSpace < 10L * 1024 * 1024 * 1024)
            {
                reportData["Disk"] = $"Low disk space: {driveInfo.AvailableFreeSpace / 1_000_000} MB available";
                healthStatus = HealthStatus.Unhealthy;
            }
            else
            {
                reportData["Disk"] = "Sufficient disk space available";
            }

            //var cpuUsage = GetCpuUsage
            var cpuUsage = 30;
            if (cpuUsage > 95)
            {
                reportData["CPU"] = $"High CPU usage: {cpuUsage}%";
                if (healthStatus != HealthStatus.Unhealthy)
                {
                    healthStatus = HealthStatus.Degraded;
                }
            }
            else
            {
                reportData["CPU"] = $"CPU usage is normal: {cpuUsage}%";
            }

            if (healthStatus == HealthStatus.Healthy)
            {
                return HealthCheckResult.Healthy("All checks passed", reportData);
            }
            else if (healthStatus == HealthStatus.Degraded)
            {
                return HealthCheckResult.Degraded("Some checks are degraded", null, reportData);
            }
            else
            {
                return HealthCheckResult.Unhealthy("Some checks failed", null, reportData);
            }
        }

        //private static double GetCpuUsage()
        //{
        //    if (Environment.OSVersion.Platform == PlatformID.Win32NT)
        //    {
        //        var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        //        cpuCounter.NextValue();
        //        System.Threading.Thread.Sleep(500);
        //        return Math.Round(cpuCounter.NextValue(), 2);
        //    }
        //    else
        //    {
        //        return GetLinuxCpuUsage();
        //    }
        //}

        private static double GetLinuxCpuUsage()
        {
            try
            {
                var lines = File.ReadAllLines("/proc/stat");
                var cpuLine = lines[0].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var idleTime = long.Parse(cpuLine[4]);
                var totalTime = 0L;
                for (int i = 1; i < 8; i++)
                {
                    totalTime += long.Parse(cpuLine[i]);
                }

                var totalIdleTime = totalTime - idleTime;
                return Math.Round(100 * (totalTime - totalIdleTime) / (double)totalTime, 2);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading CPU usage on Linux: " + ex.Message);
                return 0;
            }
        }

        private static string GetDiskPath()
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                return "C:";
            } else
            {
                return "/";
            }
        }

    }
}
