#!/usr/bin/python3

import os
import time

publish_path = '/Users/wanglihua/files/publish/shunda'
client_publish_path = publish_path + '/client'
shunda_path = publish_path + '/shunda'

os.system('dotnet publish /Users/wanglihua/projects/shunda/Client/P04_Client_Publish.csproj --no-restore -c Release -o ' + client_publish_path)

os.system('rm -rf {shunda_path}'.format(shunda_path=shunda_path))

os.system('mkdir {shunda_path}'.format(shunda_path=shunda_path))
os.system('mkdir {shunda_path}/client'.format(shunda_path=shunda_path))

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

os.system('rm -f {publish_path}/shunda_client.7z'.format(publish_path=publish_path))
os.system('7z a {publish_path}/shunda_client.7z {publish_path}/shunda'.format(publish_path=publish_path))
os.system('cp -f {publish_path}/shunda_client.7z /Users/wanglihua/Downloads/RemoteFolder/shunda_client_{timeFormatStr}.7z'.format(publish_path=publish_path, timeFormatStr=time.strftime("%Y%m%d%H%M", time.localtime())))
