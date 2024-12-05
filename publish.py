import os
import sys
from datetime import datetime

solution_path = "d:\Projects\shunda"
publish_path = "d:\Downloads\RemoteFolder\publish\shunda"
pack_path = publish_path + "\shunda"

# import shutil
# shutil.rmtree(pack_path)
os.system('rmdir "{pack_path}" /s /q'.format(pack_path=pack_path))
os.system('mkdir "{pack_path}"'.format(pack_path=pack_path))

current_date_time = datetime.now().strftime("%Y%m%d%H%M")

argv = sys.argv
argv_len = len(sys.argv)

if argv_len == 1 or argv[1] == "admin":
    os.system('mkdir "{pack_path}\admin"'.format(pack_path=pack_path))
    
    admin_proj_file_full_name = "{solution_path}\Admin\Admin.csproj".format(solution_path=solution_path)
    with open(admin_proj_file_full_name, "r", encoding="utf-8") as f:
        admin_proj_file_content = f.read()
    admin_publish_proj_file_full_name = "{solution_path}\Admin\Admin_publish.csproj".format(solution_path=solution_path)
    with open(admin_publish_proj_file_full_name, "w", encoding="utf-8") as f:
        f.write(admin_proj_file_content.replace("<RazorCompileOnBuild>false</RazorCompileOnBuild>", "<RazorCompileOnBuild>true</RazorCompileOnBuild><MvcRazorCompileOnPublish>true</MvcRazorCompileOnPublish>"))
    os.system('dotnet publish "{solution_path}\Admin\Admin_publish.csproj" --no-restore -c Release -o "{publish_path}\admin"'.format(solution_path=solution_path, publish_path=publish_path))
    os.system('del "{solution_path}\Admin\Admin_publish.csproj"'.format(solution_path=solution_path))
    
    os.system('copy "{publish_path}\admin\shunda.Core.pdb" "{pack_path}\admin\shunda.Core.pdb" /V'.format(publish_path=publish_path, pack_path=pack_path))
    os.system('copy "{publish_path}\admin\shunda.Core.dll" "{pack_path}\admin\shunda.Core.dll" /V'.format(publish_path=publish_path, pack_path=pack_path))
    os.system('copy "{publish_path}\admin\shunda.Domain.pdb" "{pack_path}\admin\shunda.Domain.pdb" /V'.format(publish_path=publish_path, pack_path=pack_path))
    os.system('copy "{publish_path}\admin\shunda.Domain.dll" "{pack_path}\admin\shunda.Domain.dll" /V'.format(publish_path=publish_path, pack_path=pack_path))
    os.system('copy "{publish_path}\admin\shunda.Service.pdb" "{pack_path}\admin\shunda.Service.pdb" /V'.format(publish_path=publish_path, pack_path=pack_path))
    os.system('copy "{publish_path}\admin\shunda.Service.dll" "{pack_path}\admin\shunda.Service.dll" /V'.format(publish_path=publish_path, pack_path=pack_path))
    os.system('copy "{publish_path}\admin\shunda.Admin.exe" "{pack_path}\admin\shunda.Admin.exe" /V'.format(publish_path=publish_path, pack_path=pack_path))
    os.system('copy "{publish_path}\admin\shunda.Admin.pdb" "{pack_path}\admin\shunda.Admin.pdb" /V'.format(publish_path=publish_path, pack_path=pack_path))
    os.system('copy "{publish_path}\admin\shunda.Admin.dll" "{pack_path}\admin\shunda.Admin.dll" /V'.format(publish_path=publish_path, pack_path=pack_path))
    os.system('copy "{publish_path}\admin\shunda.Admin.deps.json" "{pack_path}\admin\shunda.Admin.deps.json" /V'.format(publish_path=publish_path, pack_path=pack_path))
    os.system('copy "{publish_path}\admin\shunda.Admin.runtimeconfig.json" "{pack_path}/admin/shunda.Admin.runtimeconfig.json" /V'.format(publish_path=publish_path, pack_path=pack_path))
    
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
    
    os.system('copy "{publish_path}\job\shunda.Core.pdb" "{pack_path}\job\shunda.Core.pdb" /V'.format(publish_path=publish_path, pack_path=pack_path))
    os.system('copy "{publish_path}\job\shunda.Core.dll" "{pack_path}\job\shunda.Core.dll" /V'.format(publish_path=publish_path, pack_path=pack_path))
    os.system('copy "{publish_path}\job\shunda.Domain.pdb" "{pack_path}\job\shunda.Domain.pdb" /V'.format(publish_path=publish_path, pack_path=pack_path))
    os.system('copy "{publish_path}\job\shunda.Domain.dll" "{pack_path}\job\shunda.Domain.dll" /V'.format(publish_path=publish_path, pack_path=pack_path))
    os.system('copy "{publish_path}\job\shunda.Service.pdb" "{pack_path}\job\shunda.Service.pdb" /V'.format(publish_path=publish_path, pack_path=pack_path))
    os.system('copy "{publish_path}\job\shunda.Service.dll" "{pack_path}\job\shunda.Service.dll" /V'.format(publish_path=publish_path, pack_path=pack_path))
    os.system('copy "{publish_path}\job\shunda.Job.exe" "{pack_path}\job\shunda.Job.exe" /V'.format(publish_path=publish_path, pack_path=pack_path))
    os.system('copy "{publish_path}\job\shunda.Job.pdb" "{pack_path}\job\shunda.Job.pdb" /V'.format(publish_path=publish_path, pack_path=pack_path))
    os.system('copy "{publish_path}\job\shunda.Job.dll" "{pack_path}\job\shunda.Job.dll" /V'.format(publish_path=publish_path, pack_path=pack_path))
    os.system('copy "{publish_path}\job\shunda.Job.deps.json" "{pack_path}\job\shunda.Job.deps.json" /V'.format(publish_path=publish_path, pack_path=pack_path))
    os.system('copy "{publish_path}\job\shunda.Job.runtimeconfig.json" "{pack_path}\job\shunda.Job.runtimeconfig.json" /V'.format(publish_path=publish_path, pack_path=pack_path))
    
if argv_len == 1 or argv[1] == "client":
    os.system('mkdir "{pack_path}\client"'.format(pack_path=pack_path))
    
    client_proj_file_full_name = "{solution_path}\Client\Client.csproj".format(solution_path=solution_path)
    with open(client_proj_file_full_name, "r", encoding="utf-8") as f:
        client_proj_file_content = f.read()
    client_publish_proj_file_full_name = "{solution_path}\Client\Client_publish.csproj".format(solution_path=solution_path)
    with open(client_publish_proj_file_full_name, "w", encoding="utf-8") as f:
        f.write(client_proj_file_content.replace("<RazorCompileOnBuild>false</RazorCompileOnBuild>", "<RazorCompileOnBuild>true</RazorCompileOnBuild><MvcRazorCompileOnPublish>true</MvcRazorCompileOnPublish>"))
    os.system('dotnet publish "{solution_path}\Client\Client_publish.csproj" --no-restore -c Release -o "{publish_path}\client"'.format(solution_path=solution_path, publish_path=publish_path))
    os.system('del "{solution_path}\Client\Client_publish.csproj"'.format(solution_path=solution_path))
    
    os.system('copy "{publish_path}\client\shunda.Core.pdb" "{pack_path}\client\shunda.Core.pdb" /V'.format(publish_path=publish_path, pack_path=pack_path))
    os.system('copy "{publish_path}\client\shunda.Core.dll" "{pack_path}\client\shunda.Core.dll" /V'.format(publish_path=publish_path, pack_path=pack_path))
    os.system('copy "{publish_path}\client\shunda.Domain.pdb" "{pack_path}\client\shunda.Domain.pdb" /V'.format(publish_path=publish_path, pack_path=pack_path))
    os.system('copy "{publish_path}\client\shunda.Domain.dll" "{pack_path}\client\shunda.Domain.dll" /V'.format(publish_path=publish_path, pack_path=pack_path))
    os.system('copy "{publish_path}\client\shunda.Service.pdb" "{pack_path}\client\shunda.Service.pdb" /V'.format(publish_path=publish_path, pack_path=pack_path))
    os.system('copy "{publish_path}\client\shunda.Service.dll" "{pack_path}\client\shunda.Service.dll" /V'.format(publish_path=publish_path, pack_path=pack_path))
    os.system('copy "{publish_path}\client\shunda.Client.exe" "{pack_path}\client\shunda.Client.exe" /V'.format(publish_path=publish_path, pack_path=pack_path))
    os.system('copy "{publish_path}\client\shunda.Client.pdb" "{pack_path}\client\shunda.Client.pdb" /V'.format(publish_path=publish_path, pack_path=pack_path))
    os.system('copy "{publish_path}\client\shunda.Client.dll" "{pack_path}\client\shunda.Client.dll" /V'.format(publish_path=publish_path, pack_path=pack_path))
    os.system('copy "{publish_path}\client\shunda.Client.deps.json" "{pack_path}\client\shunda.Client.deps.json" /V'.format(publish_path=publish_path, pack_path=pack_path))
    os.system('copy "{publish_path}\client\shunda.Client.runtimeconfig.json" "{pack_path}\client\shunda.Client.runtimeconfig.json" /V'.format(publish_path=publish_path, pack_path=pack_path))
    
os.system('del "{publish_path}\shunda.7z"'.format(publish_path=publish_path))
os.system('7z a "{publish_path}\shunda.7z" "{pack_path}"'.format(publish_path=publish_path, pack_path=pack_path))

suffix = ""
if argv_len > 1:
    suffix = "_" + argv[1]

os.system('copy "{publish_path}\shunda.7z" "d:\Downloads\RemoteFolder\shunda{suffix}_{current_date_time}.7z"'.format(publish_path=publish_path, suffix=suffix, current_date_time=current_date_time))

print("publish complete!")