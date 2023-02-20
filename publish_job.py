#!/usr/bin/python3

import os
import time

publish_path = '/Users/wanglihua/files/publish/shunda'
job_publish_path = publish_path + '/job'
shunda_path = publish_path + '/shunda'

os.system('dotnet publish /Users/wanglihua/projects/shunda/Job/P06_Job_Publish.csproj --no-restore -c Release -o ' + job_publish_path)

os.system('rm -rf {shunda_path}'.format(shunda_path=shunda_path))

os.system('mkdir {shunda_path}'.format(shunda_path=shunda_path))
os.system('mkdir {shunda_path}/job'.format(shunda_path=shunda_path))

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

os.system('rm -f {publish_path}/shunda_job.7z'.format(publish_path=publish_path))
os.system('7z a {publish_path}/shunda_job.7z {publish_path}/shunda'.format(publish_path=publish_path))
os.system('cp -f {publish_path}/shunda_job.7z /Users/wanglihua/Downloads/RemoteFolder/shunda_job_{timeFormatStr}.7z'.format(publish_path=publish_path, timeFormatStr=time.strftime("%Y%m%d%H%M", time.localtime())))
