pack_admin=test [ "$1" = "" ] || [ "$1" = "admin" ]
pack_job=test [ "$1" = "" ] || [ "$1" = "job" ]
pack_client=test [ "$1" = "" ] || [ "$1" = "client" ]

solution_path="/Users/wanglihua/projects/shunda"
publish_path="/Users/wanglihua/files/publish/shunda"
pack_path="${publish_path}/shunda"

rm -rf ${pack_path}

mkdir ${pack_path}

if [ "$1" = "" ] || [ "$1" = "admin" ]; then
    mkdir ${pack_path}/admin

    project_file_content=$(<${solution_path}/Admin/P01_Admin.csproj)
    project_file_content=${project_file_content/<RazorCompileOnBuild>false<\/RazorCompileOnBuild>/<RazorCompileOnBuild>true<\/RazorCompileOnBuild><MvcRazorCompileOnPublish>true<\/MvcRazorCompileOnPublish>}
    # rm ${solution_path}/Admin/P01_Admin_publish.csproj
    echo ${project_file_content} >> ${solution_path}/Admin/P01_Admin_publish.csproj
    dotnet publish ${solution_path}/Admin/P01_Admin_publish.csproj --no-restore -c Release -o ${publish_path}/admin
    rm ${solution_path}/Admin/P01_Admin_publish.csproj
    
    publish_admin_path="${publish_path}/admin"
    cp -f ${publish_path}/admin/shunda.Core.pdb ${pack_path}/admin/shunda.Core.pdb
    cp -f ${publish_path}/admin/shunda.Core.dll ${pack_path}/admin/shunda.Core.dll
    cp -f ${publish_path}/admin/shunda.Domain.pdb ${pack_path}/admin/shunda.Domain.pdb
    cp -f ${publish_path}/admin/shunda.Domain.dll ${pack_path}/admin/shunda.Domain.dll
    cp -f ${publish_path}/admin/shunda.Service.pdb ${pack_path}/admin/shunda.Service.pdb
    cp -f ${publish_path}/admin/shunda.Service.dll ${pack_path}/admin/shunda.Service.dll
    cp -f ${publish_path}/admin/shunda.Admin.dll ${pack_path}/admin/shunda.Admin
    cp -f ${publish_path}/admin/shunda.Admin.pdb ${pack_path}/admin/shunda.Admin.pdb
    cp -f ${publish_path}/admin/shunda.Admin.dll ${pack_path}/admin/shunda.Admin.dll
    cp -f ${publish_path}/admin/shunda.Admin.deps.json ${pack_path}/admin/shunda.Admin.deps.json
    cp -f ${publish_path}/admin/shunda.Admin.runtimeconfig.json ${pack_path}/admin/shunda.Admin.runtimeconfig.json
fi

if [ "$1" = "" ] || [ "$1" = "job" ]; then
    mkdir ${pack_path}/job
    
    project_file_content=$(<${solution_path}/Job/P06_Job.csproj)
    project_file_content=${project_file_content/<RazorCompileOnBuild>false<\/RazorCompileOnBuild>/<RazorCompileOnBuild>true<\/RazorCompileOnBuild><MvcRazorCompileOnPublish>true<\/MvcRazorCompileOnPublish>}
    # rm ${solution_path}/Job/P06_Job_publish.csproj
    echo ${project_file_content} >> ${solution_path}/Job/P06_Job_publish.csproj
    dotnet publish ${solution_path}/Job/P06_Job_publish.csproj --no-restore -c Release -o ${publish_path}/job
    rm ${solution_path}/Job/P06_Job_publish.csproj
    
    publish_job_path="${publish_path}/job"
    cp -f ${publish_path}/job/shunda.Core.pdb ${pack_path}/job/shunda.Core.pdb
    cp -f ${publish_path}/job/shunda.Core.dll ${pack_path}/job/shunda.Core.dll
    cp -f ${publish_path}/job/shunda.Domain.pdb ${pack_path}/job/shunda.Domain.pdb
    cp -f ${publish_path}/job/shunda.Domain.dll ${pack_path}/job/shunda.Domain.dll
    cp -f ${publish_path}/job/shunda.Service.pdb ${pack_path}/job/shunda.Service.pdb
    cp -f ${publish_path}/job/shunda.Service.dll ${pack_path}/job/shunda.Service.dll
    cp -f ${publish_path}/job/shunda.Job.dll ${pack_path}/job/shunda.Job
    cp -f ${publish_path}/job/shunda.Job.pdb ${pack_path}/job/shunda.Job.pdb
    cp -f ${publish_path}/job/shunda.Job.dll ${pack_path}/job/shunda.Job.dll
    cp -f ${publish_path}/job/shunda.Job.deps.json ${pack_path}/job/shunda.Job.deps.json
    cp -f ${publish_path}/job/shunda.Job.runtimeconfig.json ${pack_path}/job/shunda.Job.runtimeconfig.json
fi

if [ "$1" = "" ] || [ "$1" = "client" ]; then
    mkdir ${pack_path}/client
    
    project_file_content=$(<${solution_path}/Client/P04_Client.csproj)
    project_file_content=${project_file_content/<RazorCompileOnBuild>false<\/RazorCompileOnBuild>/<RazorCompileOnBuild>true<\/RazorCompileOnBuild><MvcRazorCompileOnPublish>true<\/MvcRazorCompileOnPublish>}
    # rm ${solution_path}/Client/P04_Client_publish.csproj
    echo ${project_file_content} >> ${solution_path}/Client/P04_Client_publish.csproj
    dotnet publish ${solution_path}/Client/P04_Client_publish.csproj --no-restore -c Release -o ${publish_path}/client
    rm ${solution_path}/Client/P04_Client_publish.csproj
    
    cp -f ${publish_path}/client/shunda.Core.pdb ${pack_path}/client/shunda.Core.pdb
    cp -f ${publish_path}/client/shunda.Core.dll ${pack_path}/client/shunda.Core.dll
    cp -f ${publish_path}/client/shunda.Domain.pdb ${pack_path}/client/shunda.Domain.pdb
    cp -f ${publish_path}/client/shunda.Domain.dll ${pack_path}/client/shunda.Domain.dll
    cp -f ${publish_path}/client/shunda.Service.pdb ${pack_path}/client/shunda.Service.pdb
    cp -f ${publish_path}/client/shunda.Service.dll ${pack_path}/client/shunda.Service.dll
    cp -f ${publish_path}/client/shunda.Client.dll ${pack_path}/client/shunda.Client
    cp -f ${publish_path}/client/shunda.Client.pdb ${pack_path}/client/shunda.Client.pdb
    cp -f ${publish_path}/client/shunda.Client.dll ${pack_path}/client/shunda.Client.dll
    cp -f ${publish_path}/client/shunda.Client.deps.json ${pack_path}/client/shunda.Client.deps.json
    cp -f ${publish_path}/client/shunda.Client.runtimeconfig.json ${pack_path}/client/shunda.Client.runtimeconfig.json
fi

rm -f ${publish_path}/shunda.7z
7z a ${publish_path}/shunda.7z ${pack_path}

current_date_time="`date +%Y%m%d%H%M%S`";
cp -f ${publish_path}/shunda.7z /Users/wanglihua/Downloads/RemoteFolder/shunda_${current_date_time}.7z

echo "publish complete!"