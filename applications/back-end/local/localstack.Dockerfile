FROM localstack/localstack

#in case 'init-aws.sh' has been modified then 'back-end-localstack' this custom
#localstack image has to be rebuit to rerun the bash init script
#and therefore reflect any changes made to the script  
COPY --chown=localstack ./init-aws.sh /etc/localstack/init/ready.d/init-aws.sh

#!!!to make sure localstack has access to init-aws.sh to execute init script!!! 
#https://github.com/localstack/localstack/issues/7596
#https://docs.localstack.cloud/references/init-hooks/
RUN chmod u+x /etc/localstack/init/ready.d/init-aws.sh