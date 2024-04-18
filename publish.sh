solution_path="/Users/wanglihua/projects/shunda"
publish_root_path="/Users/wanglihua/files/publish/shunda"
publish_admin_path="${publish_root_path}/admin"
pack_path="${publish_root_path}/shunda"

rm -rf ${pack_path}

mkdir ${pack_path}
mkdir ${pack_path}/admin

admin_project_file_content=$(<${solution_path}/Admin/P01_Admin.csproj)
admin_project_file_content=${admin_project_file_content/<RazorCompileOnBuild>false<\/RazorCompileOnBuild>/<RazorCompileOnBuild>true<\/RazorCompileOnBuild><MvcRazorCompileOnPublish>true<\/MvcRazorCompileOnPublish>}
rm ${solution_path}/Admin/P01_Admin_publish.csproj
echo ${admin_project_file_content} >> ${solution_path}/Admin/P01_Admin_publish.csproj
dotnet publish ${solution_path}/Admin/P01_Admin_publish.csproj --no-restore -c Release -o ${publish_admin_path}
rm ${solution_path}/Admin/P01_Admin_publish.csproj

cp -f ${publish_admin_path}/shunda.Core.pdb ${pack_path}/admin/shunda.Core.pdb
cp -f ${publish_admin_path}/shunda.Core.dll ${pack_path}/admin/shunda.Core.dll
cp -f ${publish_admin_path}/shunda.Domain.pdb ${pack_path}/admin/shunda.Domain.pdb
cp -f ${publish_admin_path}/shunda.Domain.dll ${pack_path}/admin/shunda.Domain.dll
cp -f ${publish_admin_path}/shunda.Service.pdb ${pack_path}/admin/shunda.Service.pdb
cp -f ${publish_admin_path}/shunda.Service.dll ${pack_path}/admin/shunda.Service.dll
cp -f ${publish_admin_path}/shunda.Admin.dll ${pack_path}/admin/shunda.Admin
cp -f ${publish_admin_path}/shunda.Admin.pdb ${pack_path}/admin/shunda.Admin.pdb
cp -f ${publish_admin_path}/shunda.Admin.dll ${pack_path}/admin/shunda.Admin.dll
cp -f ${publish_admin_path}/shunda.Admin.deps.json ${pack_path}/admin/shunda.Admin.deps.json
cp -f ${publish_admin_path}/shunda.Admin.runtimeconfig.json ${pack_path}/admin/shunda.Admin.runtimeconfig.json


rm -f ${publish_root_path}/shunda.7z
7z a ${publish_root_path}/shunda.7z ${pack_path}

current_date_time="`date +%Y%m%d%H%M%S`";
cp -f ${publish_root_path}/shunda.7z /Users/wanglihua/Downloads/RemoteFolder/shunda_${current_date_time}.7z

echo "publish complete!"
