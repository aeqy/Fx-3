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


---
## 提交类型指定为下面其中一个：


| 提交类型 | 描述说明 |
|----------|----------|
| `build`  | 对构建系统或者外部依赖项进行了修改 |
| `chore`  | 对构建过程或辅助工具和库的更改 |
| `ci`     | 对CI配置文件或脚本进行了修改 |
| `docs`   | 对文档进行了修改 |
| `feat`   | 增加新的特征 |
| `fix`    | 修复bug |
| `pref`   | 提高性能的代码更改 |
| `refactor`| 既不是修复bug也不是添加特征的代码重构 |
| `style`  | 不影响代码含义的修改，比如空格、格式化、缺失的分
