abp install-libs

cd src/FogLightTask.DbMigrator && dotnet run && cd -



cd src/FogLightTask.Web && dotnet dev-certs https -v -ep openiddict.pfx -p config.auth_server_default_pass_phrase 


exit 0