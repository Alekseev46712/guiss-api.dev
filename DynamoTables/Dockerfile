FROM hashicorp/terraform
RUN mkdir /terraform
WORKDIR /terraform
COPY *.tf ./
RUN terraform init
ENTRYPOINT [ "/bin/terraform", "apply", "-auto-approve" ]
