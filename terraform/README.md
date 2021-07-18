# Deployment of the Guiss API using Terraform

This directory contains [Terraform](https://www.terraform.io) modules which can be used to deploy the Guiss API.

Terraform is a command line utility which can be installed from the [official downloads page](https://www.terraform.io/downloads.html)
or via Chocolatey: `choco install terraform -y`

## Folder structure

Reference for this can be found here:
[GuissAPI Folder Structure ](https://confluence.refinitiv.com/display/PCS/Walk-through+in+Terraform+Deployment+for+the+AAA+-+API+projects).


## Manual deployment procedure

**First**: build a Lambda package for the Guiss API locally running these commands:

```
cd /path-to-your-repos/aaa-api-template/Refinitiv.Aaa.GuissApi
```

```
dotnet lambda package --configuration Release
```

This package should now exist under: `path-to-your-repo/aaa-api-template/Refinitiv.Aaa.GuissApi/bin/Release/netcoreapp2.1/Refinitiv.Aaa.GuissApi.zip`


**Second**: navigate inside the infrastructure folder:

```
cd path-to-your-repo/aaa-api-template/terraform/template-api-sdlc
```

**Third**: use `cloud-tool-fr` to login in AWS and update your credentials folder; make sure you update your **aaa-sdlc-preprod** account.


**Fourth**: build your terraform infrastructure:

**NOTE: only build/run infrastructure from inside the .\environments\manual\ folder!! **

Initialize your working directory, by running the command: 

```
terraform init -backend-config="..\environments\manual\region\us-east-1\infra-backend.tfvars"
```

This command should have created a `/.terraform` folder.

Assuming you are working on a feature branch called PENTITLE-XXXX, create an execution plan, by running the command:

```
terraform plan -lock=false -var-file="..\environments\manual\region\us-east-1\infra-env.tfvars" -var name_suffix=feature-PENTITLE-XXXX
```

You should be able to see all the resources that are planned to be built by terraform. Verify that the resources to be created are as expected!

Apply the changes required to reach the desired state of the configuration, specified on the plan stage by running the command:

```
terraform apply -auto-approve -lock=false -var-file="..\environments\manual\region\us-east-1\infra-env.tfvars" -var name_suffix=feature-PENTITLE-XXXX
```

Your new infrastructure resources should now be created and the outputs of them displayed.

When you are ready tear down your infrastructure, by running the command:

```
terraform destroy -auto-approve -lock=false -var-file="..\environments\manual\region\us-east-1\infra-env.tfvars" -var name_suffix=feature-PENTITLE-XXXX
```

*Notes:* 

1. This will leave you with a `/.terraform` folder, `terraform.tfstate` and `terraform.tfstate.backup` files. If you repeat the terraform commands, `terraform init` is not needed since it will be initialized with the cached information.
2. If you want to run a new clean terraform deployment remove this folder and those two files and run again `terraform init` and repeat the steps.
3. You might noticed that we chose to not lock the state, this is because AWS credentials expire and only one person should be working on the manual deployment.



## Rollback - Troubleshoot 

If there is a problem at any point e.g terraform commands hang or AWS credentials expire make sure you get valid credentials and re-run `terraform plan ....`. This will give a hint of what is going on with the current state of your resources and infrastructure. 

Since you have been running all terraform commands without locking the state you will always have access to your infrastructure.

Make sure you delete everything that is stuck or hanging. If needed delete resources manually as a last resort. 

