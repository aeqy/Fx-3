# Fx

## Clean architecture

### 项目结构

#### 项目结构：展示分层结构和依赖关系。
##### 领域层（Domain Layer）：定义核心业务逻辑和实体。
##### 应用层（Application Layer）：定义用例和服务。
##### 基础设施层（Infrastructure Layer）：配置 OpenIddict 和实现身份验证逻辑。
##### 接口层（Presentation Layer）：处理用户请求。


### 测试环境自签证书
` openssl req -x509 -newkey rsa:2048 -keyout mydomain.key -out mydomain.crt -days 365 -nodes -subj "/C=CN/ST=Beijing/L=Beijing/O=MyCompany/OU=IT/CN=mydomain.com" && openssl pkcs12 -export -out mydomain.pfx -inkey mydomain.key -in mydomain.crt -passout pass:mypassword `

### 添加 OpenIddict 支持

#### 测试接口 

    POST /connect/token 
    ontent-Type: application/x-www-form-urlencoded
    grant_type=password&username=admin&password=Admin@123&client_id=my-client
