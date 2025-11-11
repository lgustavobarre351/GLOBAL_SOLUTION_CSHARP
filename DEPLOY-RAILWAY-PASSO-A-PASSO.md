# 🚀 Deploy no Railway - Passo a Passo Completo

## ✅ PRÉ-REQUISITOS

- [x] Código no GitHub (este repositório)
- [x] Conta no Railway.app (criar em https://railway.app)
- [x] Conta no GitHub

---

## 📋 PASSO 1: PREPARAR O REPOSITÓRIO

### 1.1 Verificar arquivos essenciais

Certifique-se de que estes arquivos estão na raiz do projeto:

- ✅ `railway.json` - Configuração do Railway
- ✅ `Entrevistas/Dockerfile` - Build da aplicação
- ✅ `Entrevistas/Program.cs` - Lê `DATABASE_URL` automaticamente

### 1.2 Fazer commit e push

```bash
# Se ainda não fez commit das alterações
git add .
git commit -m "Configurar para deploy no Railway"
git push origin main
```

---

## 🚂 PASSO 2: CRIAR PROJETO NO RAILWAY

### 2.1 Acessar Railway

1. Acesse https://railway.app
2. Clique em **"Login"**
3. Escolha **"Login with GitHub"**
4. Autorize o Railway a acessar seus repositórios

### 2.2 Criar novo projeto

1. Clique em **"New Project"**
2. Escolha **"Deploy from GitHub repo"**
3. Selecione o repositório: **`GLOBAL_SOLUTION_CSHARP`**
4. Aguarde Railway detectar o projeto (vai mostrar Dockerfile)
5. Clique em **"Deploy Now"**

> ⏱️ **Primeiro deploy vai falhar** - é normal! Falta configurar o banco de dados.

---

## 🗄️ PASSO 3: ADICIONAR POSTGRESQL

### 3.1 Adicionar banco

1. No dashboard do projeto, clique em **"+ New"**
2. Escolha **"Database"**
3. Selecione **"Add PostgreSQL"**
4. Aguarde 1-2 minutos para criação

### 3.2 Verificar variáveis automáticas

Railway cria automaticamente:

- ✅ `DATABASE_URL` - Connection string completa
- ✅ `PGHOST`, `PGPORT`, `PGUSER`, `PGPASSWORD`, `PGDATABASE`

> 💡 Seu `Program.cs` já está configurado para ler `DATABASE_URL` automaticamente!

---

## 🔄 PASSO 4: APLICAR MIGRATIONS (CRUCIAL!)

### Opção A: Via Railway CLI (Recomendado)

```bash
# 1. Instalar Railway CLI
npm i -g @railway/cli

# 2. Fazer login
railway login

# 3. Vincular ao projeto
railway link

# 4. Aplicar migrations
railway run dotnet ef database update --project Entrevistas
```

### Opção B: Manualmente via Variável de Ambiente

1. No Railway, clique no serviço **PostgreSQL**
2. Vá em **"Variables"** → Copie o valor de `DATABASE_URL`
3. No terminal local:

```bash
# Windows PowerShell
$env:DATABASE_URL="postgresql://postgres:senha@host:port/railway"
dotnet ef database update --project Entrevistas

# Linux/Mac
export DATABASE_URL="postgresql://postgres:senha@host:port/railway"
dotnet ef database update --project Entrevistas
```

### Opção C: Conectar via Supabase (Seu caso atual)

Se você já tem o banco criado no Supabase:

1. No Railway, clique no serviço da **API** (não no PostgreSQL)
2. Vá em **"Variables"**
3. Clique em **"+ New Variable"**
4. Adicione:
   - **Name:** `DATABASE_URL`
   - **Value:** `sua-connection-string-do-supabase`
5. Clique em **"Deploy"** para reiniciar

---

## ⚙️ PASSO 5: CONFIGURAR VARIÁVEIS (OPCIONAL)

As variáveis essenciais são criadas automaticamente, mas você pode adicionar:

1. Clique no serviço da **API**
2. Aba **"Variables"**
3. Adicionar (opcional):

```
ASPNETCORE_ENVIRONMENT=Production
```

> 💡 **NÃO precisa** adicionar `PORT` - Railway injeta automaticamente!

---

## 🌐 PASSO 6: OBTER URL DA API

### 6.1 Gerar domínio público

1. Clique no serviço da **API**
2. Vá em **"Settings"**
3. Na seção **"Networking"**, clique em **"Generate Domain"**
4. Railway vai gerar algo como: `sua-api.up.railway.app`

### 6.2 Testar a API

```bash
# Testar Swagger
https://sua-api.up.railway.app/swagger

# Testar endpoint de saúde
https://sua-api.up.railway.app/api/v1/test-connection
```

---

## 🔍 PASSO 7: VERIFICAR LOGS

### 7.1 Ver logs do deploy

1. Clique no serviço da **API**
2. Aba **"Deployments"**
3. Clique no deploy mais recente
4. Veja os logs em tempo real

### 7.2 O que procurar nos logs

✅ **Sucesso:**
```
🔗 Using connection: DATABASE_URL (Cloud)
✅ Conexão com banco de dados estabelecida com sucesso!
🚀 Porta: 8080
📋 Swagger: /swagger
Now listening on: http://0.0.0.0:8080
```

❌ **Erro comum:**
```
Connection string not found
```
**Solução:** Verifique se PostgreSQL está adicionado ou se `DATABASE_URL` está configurada.

---

## 🎯 CHECKLIST FINAL

Após seguir todos os passos, verifique:

- [ ] **Build bem-sucedido** - Status verde no Railway
- [ ] **PostgreSQL adicionado** - Variável `DATABASE_URL` existe
- [ ] **Migrations aplicadas** - Tabelas criadas no banco
- [ ] **Domínio gerado** - URL `*.up.railway.app` disponível
- [ ] **Swagger acessível** - `https://sua-api.up.railway.app/swagger` abre
- [ ] **Endpoints funcionando** - GET, POST, PUT, DELETE testados

---

## 🆘 TROUBLESHOOTING

### ❌ Build falha com erro de Dockerfile

**Problema:** Railway não encontra Dockerfile

**Solução:**
```bash
# Verificar se railway.json está correto
cat railway.json

# Deve mostrar:
# "dockerfilePath": "Entrevistas/Dockerfile"
```

### ❌ Erro: "Connection string not found"

**Problema:** API não consegue ler `DATABASE_URL`

**Solução:**
1. Verifique se PostgreSQL foi adicionado
2. Ou adicione manualmente a variável `DATABASE_URL` do Supabase

### ❌ Erro: "relation 'candidates' already exists"

**Problema:** Tentou aplicar migrations em banco que já tem tabelas

**Solução:**
```bash
# Opção 1: Marcar migration como aplicada (se tabelas já existem)
railway run dotnet ef database update --project Entrevistas 0
railway run dotnet ef database update --project Entrevistas

# Opção 2: Usar banco existente (Supabase)
# Adicionar DATABASE_URL do Supabase nas variáveis
```

### ❌ API fica restartando continuamente

**Problema:** Health check falhando

**Solução:**
1. Verifique logs: clique em "Deployments" → último deploy
2. Procure erro específico
3. Geralmente é problema de conexão com banco

### ❌ Swagger não carrega

**Problema:** Rota ou porta incorreta

**Solução:**
- URL correta: `https://sua-api.up.railway.app/swagger` (com /swagger)
- Verifique se API iniciou: olhe logs

---

## 📊 MONITORAMENTO

### Ver uso de recursos

1. Dashboard do Railway
2. Serviço da API → **"Metrics"**
3. Monitore:
   - CPU usage
   - Memory usage
   - Network (requests)

### Ver custo

1. Dashboard principal
2. **"Usage"** no menu lateral
3. Railway oferece:
   - ✅ **$5 USD grátis/mês**
   - ✅ **500 horas de execução**
   - ✅ Suficiente para API 24/7

---

## 🎉 APÓS DEPLOY BEM-SUCEDIDO

### Adicionar URL no README

Edite o `README.md`:

```markdown
## 🌐 Deploy em Produção

A API está disponível em: **https://sua-api.up.railway.app**

- **Swagger:** https://sua-api.up.railway.app/swagger
- **Health Check:** https://sua-api.up.railway.app/api/v1/test-connection
```

### Testar todos os endpoints

Use Swagger ou Postman:

1. **Criar empregador:** POST `/api/v1/empregadores`
2. **Criar candidato:** POST `/api/v1/candidatos`
3. **Agendar entrevista:** POST `/api/v1/entrevistas`
4. **Ver dashboard:** GET `/api/v1/entrevistas/dashboard`

---

## 📞 SUPORTE

- **Documentação Railway:** https://docs.railway.app
- **Discord Railway:** https://discord.gg/railway
- **GitHub Issues:** https://github.com/railwayapp/railway/issues

---

## ✅ RESUMO - 5 PASSOS RÁPIDOS

```
1️⃣ Push código no GitHub
2️⃣ Railway → New Project → Deploy from GitHub
3️⃣ Add PostgreSQL (ou usar Supabase)
4️⃣ Aplicar migrations: railway run dotnet ef database update
5️⃣ Generate Domain → Testar /swagger
```

**Tempo estimado:** 10-15 minutos

**Custo:** $0 - $2 USD/mês (dentro do free tier de $5)

---

**🚀 Boa sorte com o deploy!**
