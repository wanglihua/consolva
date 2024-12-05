import os
import sys
from datetime import datetime

solution_path = "d:\Projects\iepoch\csharp"
publish_path = "d:\Downloads\RemoteFolder\publish\iepoch"
pack_path = publish_path + "\iepoch"

# import shutil
# shutil.rmtree(pack_path)
os.system('rmdir "{pack_path}" /s /q'.format(pack_path=pack_path))
os.system('mkdir "{pack_path}"'.format(pack_path=pack_path))

current_date_time = datetime.now().strftime("%Y%m%d%H%M")

# Core\SiteVersion.cs
site_version_content = '''namespace Core;

public class SiteVersion
{
    public static string Version = "GENERATE_AT_PUBLISH";
}
'''

site_versoin_file_full_name = "{solution_path}\Core\SiteVersion.cs".format(solution_path=solution_path)

with open(site_versoin_file_full_name, "w", encoding="utf-8") as f:
    f.write(site_version_content.replace("GENERATE_AT_PUBLISH", current_date_time))

argv = sys.argv
argv_len = len(sys.argv)

if argv_len == 1 or argv[1] == "web":
    os.system('mkdir "{pack_path}\web"'.format(pack_path=pack_path))
    
    web_proj_file_full_name = "{solution_path}\Web\Web.csproj".format(solution_path=solution_path)
    with open(web_proj_file_full_name, "r", encoding="utf-8") as f:
        web_proj_file_content = f.read()
    web_publish_proj_file_full_name = "{solution_path}\Web\Web_publish.csproj".format(solution_path=solution_path)
    with open(web_publish_proj_file_full_name, "w", encoding="utf-8") as f:
        f.write(web_proj_file_content.replace("<RazorCompileOnBuild>false</RazorCompileOnBuild>", "<RazorCompileOnBuild>true</RazorCompileOnBuild><MvcRazorCompileOnPublish>true</MvcRazorCompileOnPublish>"))
    os.system('dotnet publish "{solution_path}\Web\Web_publish.csproj" --no-restore -c Release -o "{publish_path}\web"'.format(solution_path=solution_path, publish_path=publish_path))
    os.system('del "{solution_path}\Web\Web_publish.csproj"'.format(solution_path=solution_path))
    
    os.system('copy "{publish_path}\web\iepoch.Core.pdb" "{pack_path}\web\iepoch.Core.pdb" /V'.format(publish_path=publish_path, pack_path=pack_path))
    os.system('copy "{publish_path}\web\iepoch.Core.dll" "{pack_path}\web\iepoch.Core.dll" /V'.format(publish_path=publish_path, pack_path=pack_path))
    os.system('copy "{publish_path}\web\iepoch.Domain.pdb" "{pack_path}\web\iepoch.Domain.pdb" /V'.format(publish_path=publish_path, pack_path=pack_path))
    os.system('copy "{publish_path}\web\iepoch.Domain.dll" "{pack_path}\web\iepoch.Domain.dll" /V'.format(publish_path=publish_path, pack_path=pack_path))
    os.system('copy "{publish_path}\web\iepoch.Service.pdb" "{pack_path}\web\iepoch.Service.pdb" /V'.format(publish_path=publish_path, pack_path=pack_path))
    os.system('copy "{publish_path}\web\iepoch.Service.dll" "{pack_path}\web\iepoch.Service.dll" /V'.format(publish_path=publish_path, pack_path=pack_path))
    os.system('copy "{publish_path}\web\iepoch.Web.exe" "{pack_path}\web\iepoch.Web.exe" /V'.format(publish_path=publish_path, pack_path=pack_path))
    os.system('copy "{publish_path}\web\iepoch.Web.pdb" "{pack_path}\web\iepoch.Web.pdb" /V'.format(publish_path=publish_path, pack_path=pack_path))
    os.system('copy "{publish_path}\web\iepoch.Web.dll" "{pack_path}\web\iepoch.Web.dll" /V'.format(publish_path=publish_path, pack_path=pack_path))
    os.system('copy "{publish_path}\web\iepoch.Web.deps.json" "{pack_path}\web\iepoch.Web.deps.json" /V'.format(publish_path=publish_path, pack_path=pack_path))
    os.system('copy "{publish_path}\web\iepoch.Web.runtimeconfig.json" "{pack_path}/web/iepoch.Web.runtimeconfig.json" /V'.format(publish_path=publish_path, pack_path=pack_path))
    
if argv_len == 1 or argv[1] == "job":
    os.system('mkdir "{pack_path}\job"'.format(pack_path=pack_path))
    
    job_proj_file_full_name = "{solution_path}\Job\Job.csproj".format(solution_path=solution_path)
    with open(job_proj_file_full_name, "r", encoding="utf-8") as f:
        job_proj_file_content = f.read()
    job_publish_proj_file_full_name = "{solution_path}\Job\Job_publish.csproj".format(solution_path=solution_path)
    with open(job_publish_proj_file_full_name, "w", encoding="utf-8") as f:
        f.write(job_proj_file_content.replace("<RazorCompileOnBuild>false</RazorCompileOnBuild>", "<RazorCompileOnBuild>true</RazorCompileOnBuild><MvcRazorCompileOnPublish>true</MvcRazorCompileOnPublish>"))
    os.system('dotnet publish "{solution_path}\Job\Job_publish.csproj" --no-restore -c Release -o "{publish_path}\job"'.format(solution_path=solution_path, publish_path=publish_path))
    os.system('del "{solution_path}\Job\Job_publish.csproj"'.format(solution_path=solution_path))
    
    os.system('copy "{publish_path}\job\iepoch.Core.pdb" "{pack_path}\job\iepoch.Core.pdb" /V'.format(publish_path=publish_path, pack_path=pack_path))
    os.system('copy "{publish_path}\job\iepoch.Core.dll" "{pack_path}\job\iepoch.Core.dll" /V'.format(publish_path=publish_path, pack_path=pack_path))
    os.system('copy "{publish_path}\job\iepoch.Domain.pdb" "{pack_path}\job\iepoch.Domain.pdb" /V'.format(publish_path=publish_path, pack_path=pack_path))
    os.system('copy "{publish_path}\job\iepoch.Domain.dll" "{pack_path}\job\iepoch.Domain.dll" /V'.format(publish_path=publish_path, pack_path=pack_path))
    os.system('copy "{publish_path}\job\iepoch.Service.pdb" "{pack_path}\job\iepoch.Service.pdb" /V'.format(publish_path=publish_path, pack_path=pack_path))
    os.system('copy "{publish_path}\job\iepoch.Service.dll" "{pack_path}\job\iepoch.Service.dll" /V'.format(publish_path=publish_path, pack_path=pack_path))
    os.system('copy "{publish_path}\job\iepoch.Job.exe" "{pack_path}\job\iepoch.Job.exe" /V'.format(publish_path=publish_path, pack_path=pack_path))
    os.system('copy "{publish_path}\job\iepoch.Job.pdb" "{pack_path}\job\iepoch.Job.pdb" /V'.format(publish_path=publish_path, pack_path=pack_path))
    os.system('copy "{publish_path}\job\iepoch.Job.dll" "{pack_path}\job\iepoch.Job.dll" /V'.format(publish_path=publish_path, pack_path=pack_path))
    os.system('copy "{publish_path}\job\iepoch.Job.deps.json" "{pack_path}\job\iepoch.Job.deps.json" /V'.format(publish_path=publish_path, pack_path=pack_path))
    os.system('copy "{publish_path}\job\iepoch.Job.runtimeconfig.json" "{pack_path}\job\iepoch.Job.runtimeconfig.json" /V'.format(publish_path=publish_path, pack_path=pack_path))
    
if argv_len == 1 or argv[1] == "chat":
    os.system('mkdir "{pack_path}\chat"'.format(pack_path=pack_path))
    
    chat_proj_file_full_name = "{solution_path}\Chat\Chat.csproj".format(solution_path=solution_path)
    with open(chat_proj_file_full_name, "r", encoding="utf-8") as f:
        chat_proj_file_content = f.read()
    chat_publish_proj_file_full_name = "{solution_path}\Chat\Chat_publish.csproj".format(solution_path=solution_path)
    with open(chat_publish_proj_file_full_name, "w", encoding="utf-8") as f:
        f.write(chat_proj_file_content.replace("<RazorCompileOnBuild>false</RazorCompileOnBuild>", "<RazorCompileOnBuild>true</RazorCompileOnBuild><MvcRazorCompileOnPublish>true</MvcRazorCompileOnPublish>"))
    os.system('dotnet publish "{solution_path}\Chat\Chat_publish.csproj" --no-restore -c Release -o "{publish_path}\chat"'.format(solution_path=solution_path, publish_path=publish_path))
    os.system('del "{solution_path}\Chat\Chat_publish.csproj"'.format(solution_path=solution_path))
    
    os.system('copy "{publish_path}\chat\iepoch.Core.pdb" "{pack_path}\chat\iepoch.Core.pdb" /V'.format(publish_path=publish_path, pack_path=pack_path))
    os.system('copy "{publish_path}\chat\iepoch.Core.dll" "{pack_path}\chat\iepoch.Core.dll" /V'.format(publish_path=publish_path, pack_path=pack_path))
    os.system('copy "{publish_path}\chat\iepoch.Domain.pdb" "{pack_path}\chat\iepoch.Domain.pdb" /V'.format(publish_path=publish_path, pack_path=pack_path))
    os.system('copy "{publish_path}\chat\iepoch.Domain.dll" "{pack_path}\chat\iepoch.Domain.dll" /V'.format(publish_path=publish_path, pack_path=pack_path))
    os.system('copy "{publish_path}\chat\iepoch.Service.pdb" "{pack_path}\chat\iepoch.Service.pdb" /V'.format(publish_path=publish_path, pack_path=pack_path))
    os.system('copy "{publish_path}\chat\iepoch.Service.dll" "{pack_path}\chat\iepoch.Service.dll" /V'.format(publish_path=publish_path, pack_path=pack_path))
    os.system('copy "{publish_path}\chat\iepoch.Chat.exe" "{pack_path}\chat\iepoch.Chat.exe" /V'.format(publish_path=publish_path, pack_path=pack_path))
    os.system('copy "{publish_path}\chat\iepoch.Chat.pdb" "{pack_path}\chat\iepoch.Chat.pdb" /V'.format(publish_path=publish_path, pack_path=pack_path))
    os.system('copy "{publish_path}\chat\iepoch.Chat.dll" "{pack_path}\chat\iepoch.Chat.dll" /V'.format(publish_path=publish_path, pack_path=pack_path))
    os.system('copy "{publish_path}\chat\iepoch.Chat.deps.json" "{pack_path}\chat\iepoch.Chat.deps.json" /V'.format(publish_path=publish_path, pack_path=pack_path))
    os.system('copy "{publish_path}\chat\iepoch.Chat.runtimeconfig.json" "{pack_path}\chat\iepoch.Chat.runtimeconfig.json" /V'.format(publish_path=publish_path, pack_path=pack_path))

# 恢复 Core\SiteVersion.cs 原内容
with open(site_versoin_file_full_name, "w", encoding="utf-8") as f:
    f.write(site_version_content)
    
os.system('del "{publish_path}\iepoch.7z"'.format(publish_path=publish_path))
os.system('7z a "{publish_path}\iepoch.7z" "{pack_path}"'.format(publish_path=publish_path, pack_path=pack_path))

suffix = ""
if argv_len > 1:
    suffix = "_" + argv[1]

os.system('copy "{publish_path}\iepoch.7z" "d:\Downloads\RemoteFolder\iepoch{suffix}_{current_date_time}.7z"'.format(publish_path=publish_path, suffix=suffix, current_date_time=current_date_time))

print("publish complete!")