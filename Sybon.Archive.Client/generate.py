import subprocess, time, signal
p = subprocess.Popen(["dotnet", "run", "--project", "Sybon.Archive"])
time.sleep(5)
subprocess.call(["java", "-jar", ".\\Sybon.Archive.Client\\swagger-codegen-cli-3.0.0-20170904.171256-3.jar", "generate", "-l", "csharp", "--additional-properties", "targetFramework=v5.0,netCoreProjectFile=true,packageName=Sybon.Archive.Client", "-i", "http://localhost:5000/swagger/v1/swagger.json", "-o", "Sybon.Archive.Client\\sybon.archive.client"])
p.send_signal(signal.CTRL_C_EVENT)
