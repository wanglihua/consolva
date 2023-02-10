#!/usr/bin/python3

import os
import time

publish_path = '/Users/wanglihua/files/publish/shunda'
admin_publish_path = publish_path + '/admin'
shunda_path = publish_path + '/shunda'

os.system('dotnet publish /Users/wanglihua/projects/shunda/Admin/P01_Admin_Publish.csproj -c Release -o ' + admin_publish_path)

os.system('rm -rf {shunda_path}'.format(shunda_path=shunda_path))

os.system('mkdir {shunda_path}'.format(shunda_path=shunda_path))
os.system('mkdir {shunda_path}/admin'.format(shunda_path=shunda_path))

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

os.system('rm -f {publish_path}/shunda_admin.7z'.format(publish_path=publish_path))
os.system('7z a {publish_path}/shunda_admin.7z {publish_path}/shunda'.format(publish_path=publish_path))
os.system('cp -f {publish_path}/shunda_admin.7z /Users/wanglihua/Downloads/RemoteFolder/shunda_admin_{timeFormatStr}.7z'.format(publish_path=publish_path, timeFormatStr=time.strftime("%Y%m%d%H%M", time.localtime())))
