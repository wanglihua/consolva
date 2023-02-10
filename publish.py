#!/usr/bin/python3

import os
import time

publish_path = '/Users/wanglihua/files/publish/shunda'
admin_publish_path = publish_path + '/admin'
client_publish_path = publish_path + '/client'
job_publish_path = publish_path + '/job'
shunda_path = publish_path + '/shunda'

os.system('dotnet publish /Users/wanglihua/projects/shunda/Admin/P01_Admin_Publish.csproj -c Release -o ' + admin_publish_path)
os.system('dotnet publish /Users/wanglihua/projects/shunda/Client/P04_Client_Publish.csproj -c Release -o ' + client_publish_path)
os.system('dotnet publish /Users/wanglihua/projects/shunda/Job/P06_Job_Publish.csproj -c Release -o ' + job_publish_path)

os.system('rm -rf {shunda_path}'.format(shunda_path=shunda_path))

os.system('mkdir {shunda_path}'.format(shunda_path=shunda_path))
os.system('mkdir {shunda_path}/admin'.format(shunda_path=shunda_path))
os.system('mkdir {shunda_path}/client'.format(shunda_path=shunda_path))
os.system('mkdir {shunda_path}/job'.format(shunda_path=shunda_path))

os.system('cp -f {admin_publish_path}/shunda.Core.pdb {shunda_path}/admin/shunda.Core.pdb'.format(admin_publish_path=admin_publish_path, shunda_path=shunda_path))
os.system('cp -f {admin_publish_path}/shunda.Core.dll {shunda_path}/admin/shunda.Core.dll'.format(admin_publish_path=admin_publish_path, shunda_path=shunda_path))
os.system('cp -f {admin_publish_path}/shunda.Domain.pdb {shunda_path}/admin/shunda.Domain.pdb'.format(admin_publish_path=admin_publish_path, shunda_path=shunda_path))
os.system('cp -f {admin_publish_path}/shunda.Domain.dll {shunda_path}/admin/shunda.Domain.dll'.format(admin_publish_path=admin_publish_path, shunda_path=shunda_path))
os.system('cp -f {admin_publish_path}/shunda.Service.pdb {shunda_path}/admin/shunda.Service.pdb'.format(admin_publish_path=admin_publish_path, shunda_path=shunda_path))
os.system('cp -f {admin_publish_path}/shunda.Service.dll {shunda_path}/admin/shunda.Service.dll'.format(admin_publish_path=admin_publish_path, shunda_path=shunda_path))
os.system('cp -f {admin_publish_path}/shunda.Admin.dll {shunda_path}/admin/shunda.Admin'.format(admin_publish_path=admin_publish_path, shunda_path=shunda_path))
os.system('cp -f {admin_publish_path}/shunda.Admin.pdb {shunda_path}/admin/shunda.Admin.pdb'.format(admin_publish_path=admin_publish_path, shunda_path=shunda_path))
os.system('cp -f {admin_publish_path}/shunda.Admin.dll {shunda_path}/admin/shunda.Admin.dll'.format(admin_publish_path=admin_publish_path, shunda_path=shunda_path))
os.system('cp -f {admin_publish_path}/shunda.Admin.deps.json {shunda_path}/admin/shunda.Admin.deps.json'.format(admin_publish_path=admin_publish_path, shunda_path=shunda_path))
os.system('cp -f {admin_publish_path}/shunda.Admin.runtimeconfig.json {shunda_path}/admin/shunda.Admin.runtimeconfig.json'.format(admin_publish_path=admin_publish_path, shunda_path=shunda_path))

os.system('cp -f {client_publish_path}/shunda.Core.pdb {shunda_path}/client/shunda.Core.pdb'.format(client_publish_path=client_publish_path, shunda_path=shunda_path))
os.system('cp -f {client_publish_path}/shunda.Core.dll {shunda_path}/client/shunda.Core.dll'.format(client_publish_path=client_publish_path, shunda_path=shunda_path))
os.system('cp -f {client_publish_path}/shunda.Domain.pdb {shunda_path}/client/shunda.Domain.pdb'.format(client_publish_path=client_publish_path, shunda_path=shunda_path))
os.system('cp -f {client_publish_path}/shunda.Domain.dll {shunda_path}/client/shunda.Domain.dll'.format(client_publish_path=client_publish_path, shunda_path=shunda_path))
os.system('cp -f {client_publish_path}/shunda.Service.pdb {shunda_path}/client/shunda.Service.pdb'.format(client_publish_path=client_publish_path, shunda_path=shunda_path))
os.system('cp -f {client_publish_path}/shunda.Service.dll {shunda_path}/client/shunda.Service.dll'.format(client_publish_path=client_publish_path, shunda_path=shunda_path))
os.system('cp -f {client_publish_path}/shunda.Client.pdb {shunda_path}/client/shunda.Client'.format(client_publish_path=client_publish_path, shunda_path=shunda_path))
os.system('cp -f {client_publish_path}/shunda.Client.pdb {shunda_path}/client/shunda.Client.pdb'.format(client_publish_path=client_publish_path, shunda_path=shunda_path))
os.system('cp -f {client_publish_path}/shunda.Client.dll {shunda_path}/client/shunda.Client.dll'.format(client_publish_path=client_publish_path, shunda_path=shunda_path))
os.system('cp -f {client_publish_path}/shunda.Client.deps.json {shunda_path}/client/shunda.Client.deps.json'.format(client_publish_path=client_publish_path, shunda_path=shunda_path))
os.system('cp -f {client_publish_path}/shunda.Client.runtimeconfig.json {shunda_path}/client/shunda.Client.runtimeconfig.json'.format(client_publish_path=client_publish_path, shunda_path=shunda_path))

os.system('cp -f {job_publish_path}/shunda.Core.pdb {shunda_path}/job/shunda.Core.pdb'.format(job_publish_path=job_publish_path, shunda_path=shunda_path))
os.system('cp -f {job_publish_path}/shunda.Core.dll {shunda_path}/job/shunda.Core.dll'.format(job_publish_path=job_publish_path, shunda_path=shunda_path))
os.system('cp -f {job_publish_path}/shunda.Domain.pdb {shunda_path}/job/shunda.Domain.pdb'.format(job_publish_path=job_publish_path, shunda_path=shunda_path))
os.system('cp -f {job_publish_path}/shunda.Domain.dll {shunda_path}/job/shunda.Domain.dll'.format(job_publish_path=job_publish_path, shunda_path=shunda_path))
os.system('cp -f {job_publish_path}/shunda.Service.pdb {shunda_path}/job/shunda.Service.pdb'.format(job_publish_path=job_publish_path, shunda_path=shunda_path))
os.system('cp -f {job_publish_path}/shunda.Service.dll {shunda_path}/job/shunda.Service.dll'.format(job_publish_path=job_publish_path, shunda_path=shunda_path))
os.system('cp -f {job_publish_path}/shunda.Job.dll {shunda_path}/job/shunda.Job'.format(job_publish_path=job_publish_path, shunda_path=shunda_path))
os.system('cp -f {job_publish_path}/shunda.Job.pdb {shunda_path}/job/shunda.Job.pdb'.format(job_publish_path=job_publish_path, shunda_path=shunda_path))
os.system('cp -f {job_publish_path}/shunda.Job.dll {shunda_path}/job/shunda.Job.dll'.format(job_publish_path=job_publish_path, shunda_path=shunda_path))
os.system('cp -f {job_publish_path}/shunda.Job.deps.json {shunda_path}/job/shunda.Job.deps.json'.format(job_publish_path=job_publish_path, shunda_path=shunda_path))
os.system('cp -f {job_publish_path}/shunda.Job.runtimeconfig.json {shunda_path}/job/shunda.Job.runtimeconfig.json'.format(job_publish_path=job_publish_path, shunda_path=shunda_path))

os.system('rm -f {publish_path}/shunda.7z'.format(publish_path=publish_path))
os.system('7z a {publish_path}/shunda.7z {publish_path}/shunda'.format(publish_path=publish_path))
os.system('cp -f {publish_path}/shunda.7z /Users/wanglihua/Downloads/RemoteFolder/shunda_{timeFormatStr}.7z'.format(publish_path=publish_path, timeFormatStr=time.strftime("%Y%m%d%H%M", time.localtime())))
