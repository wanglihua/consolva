#!/usr/bin/python3

import os
import time

publish_path = '/Users/wanglihua/files/publish/shunda'
admin_publish_path = publish_path + '/admin'
client_publish_path = publish_path + '/client'
# chat_publish_path = publish_path + '/chat'
shunda_path = publish_path + '/shunda'

os.system('dotnet publish /Users/wanglihua/projects/shunda/Admin/P01_Admin.csproj -c Release -o ' + admin_publish_path)
os.system('dotnet publish /Users/wanglihua/projects/shunda/Client/P04_Client.csproj -c Release -o ' + client_publish_path)
# os.system('dotnet publish /Users/wanglihua/projects/shunda/csharp/Chat/Chat.csproj -c Release -o ' + chat_publish_path)

os.system('rm -rf {shunda_path}'.format(shunda_path=shunda_path))

os.system('mkdir {shunda_path}'.format(shunda_path=shunda_path))
os.system('mkdir {shunda_path}/admin'.format(shunda_path=shunda_path))
os.system('mkdir {shunda_path}/client'.format(shunda_path=shunda_path))
# os.system('mkdir {shunda_path}/chat'.format(shunda_path=shunda_path))

os.system('cp -f {admin_publish_path}/shunda.Core.pdb {shunda_path}/admin/shunda.Core.pdb'.format(admin_publish_path=admin_publish_path, shunda_path=shunda_path))
os.system('cp -f {admin_publish_path}/shunda.Core.dll {shunda_path}/admin/shunda.Core.dll'.format(admin_publish_path=admin_publish_path, shunda_path=shunda_path))
os.system('cp -f {admin_publish_path}/shunda.Domain.pdb {shunda_path}/admin/shunda.Domain.pdb'.format(admin_publish_path=admin_publish_path, shunda_path=shunda_path))
os.system('cp -f {admin_publish_path}/shunda.Domain.dll {shunda_path}/admin/shunda.Domain.dll'.format(admin_publish_path=admin_publish_path, shunda_path=shunda_path))
os.system('cp -f {admin_publish_path}/shunda.Service.pdb {shunda_path}/admin/shunda.Service.pdb'.format(admin_publish_path=admin_publish_path, shunda_path=shunda_path))
os.system('cp -f {admin_publish_path}/shunda.Service.dll {shunda_path}/admin/shunda.Service.dll'.format(admin_publish_path=admin_publish_path, shunda_path=shunda_path))
os.system('cp -f {admin_publish_path}/shunda.Admin.pdb {shunda_path}/admin/shunda.Admin.pdb'.format(admin_publish_path=admin_publish_path, shunda_path=shunda_path))
os.system('cp -f {admin_publish_path}/shunda.Admin.dll {shunda_path}/admin/shunda.Admin.dll'.format(admin_publish_path=admin_publish_path, shunda_path=shunda_path))
os.system('cp -f {admin_publish_path}/shunda.Admin.deps.json {shunda_path}/admin/shunda.Admin.deps.json'.format(admin_publish_path=admin_publish_path, shunda_path=shunda_path))

os.system('cp -f {client_publish_path}/shunda.Core.pdb {shunda_path}/client/shunda.Core.pdb'.format(client_publish_path=client_publish_path, shunda_path=shunda_path))
os.system('cp -f {client_publish_path}/shunda.Core.dll {shunda_path}/client/shunda.Core.dll'.format(client_publish_path=client_publish_path, shunda_path=shunda_path))
os.system('cp -f {client_publish_path}/shunda.Domain.pdb {shunda_path}/client/shunda.Domain.pdb'.format(client_publish_path=client_publish_path, shunda_path=shunda_path))
os.system('cp -f {client_publish_path}/shunda.Domain.dll {shunda_path}/client/shunda.Domain.dll'.format(client_publish_path=client_publish_path, shunda_path=shunda_path))
os.system('cp -f {client_publish_path}/shunda.Service.pdb {shunda_path}/client/shunda.Service.pdb'.format(client_publish_path=client_publish_path, shunda_path=shunda_path))
os.system('cp -f {client_publish_path}/shunda.Service.dll {shunda_path}/client/shunda.Service.dll'.format(client_publish_path=client_publish_path, shunda_path=shunda_path))
os.system('cp -f {client_publish_path}/shunda.Client.pdb {shunda_path}/client/shunda.Client.pdb'.format(client_publish_path=client_publish_path, shunda_path=shunda_path))
os.system('cp -f {client_publish_path}/shunda.Client.dll {shunda_path}/client/shunda.Client.dll'.format(client_publish_path=client_publish_path, shunda_path=shunda_path))
os.system('cp -f {client_publish_path}/shunda.Client.deps.json {shunda_path}/client/shunda.Client.deps.json'.format(client_publish_path=client_publish_path, shunda_path=shunda_path))

# os.system('cp -f {chat_publish_path}/shunda.Core.pdb {shunda_path}/chat/shunda.Core.pdb'.format(chat_publish_path=chat_publish_path, shunda_path=shunda_path))
# os.system('cp -f {chat_publish_path}/shunda.Core.dll {shunda_path}/chat/shunda.Core.dll'.format(chat_publish_path=chat_publish_path, shunda_path=shunda_path))
# os.system('cp -f {chat_publish_path}/shunda.Domain.pdb {shunda_path}/chat/shunda.Domain.pdb'.format(chat_publish_path=chat_publish_path, shunda_path=shunda_path))
# os.system('cp -f {chat_publish_path}/shunda.Domain.dll {shunda_path}/chat/shunda.Domain.dll'.format(chat_publish_path=chat_publish_path, shunda_path=shunda_path))
# os.system('cp -f {chat_publish_path}/shunda.Service.pdb {shunda_path}/chat/shunda.Service.pdb'.format(chat_publish_path=chat_publish_path, shunda_path=shunda_path))
# os.system('cp -f {chat_publish_path}/shunda.Service.dll {shunda_path}/chat/shunda.Service.dll'.format(chat_publish_path=chat_publish_path, shunda_path=shunda_path))
# os.system('cp -f {chat_publish_path}/shunda.Chat.pdb {shunda_path}/chat/shunda.Chat.pdb'.format(chat_publish_path=chat_publish_path, shunda_path=shunda_path))
# os.system('cp -f {chat_publish_path}/shunda.Chat.dll {shunda_path}/chat/shunda.Chat.dll'.format(chat_publish_path=chat_publish_path, shunda_path=shunda_path))
# os.system('cp -f {chat_publish_path}/shunda.Chat.PrecompiledViews.pdb {shunda_path}/chat/shunda.Chat.PrecompiledViews.pdb'.format(chat_publish_path=chat_publish_path, shunda_path=shunda_path))
# os.system('cp -f {chat_publish_path}/shunda.Chat.PrecompiledViews.dll {shunda_path}/chat/shunda.Chat.PrecompiledViews.dll'.format(chat_publish_path=chat_publish_path, shunda_path=shunda_path))
# os.system('cp -f {chat_publish_path}/shunda.Chat.deps.json {shunda_path}/chat/shunda.Chat.deps.json'.format(chat_publish_path=chat_publish_path, shunda_path=shunda_path))

os.system('rm -f {publish_path}/shunda.7z'.format(publish_path=publish_path))
os.system('7z a {publish_path}/shunda.7z {publish_path}/shunda'.format(publish_path=publish_path))
os.system('cp -f {publish_path}/shunda.7z /Users/wanglihua/Downloads/RemoteFolder/shunda_{timeFormatStr}.7z'.format(publish_path=publish_path, timeFormatStr=time.strftime("%Y%m%d%H%M", time.localtime())))
