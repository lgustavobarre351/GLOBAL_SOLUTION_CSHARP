# 🚀 Deploy Railway com Supabase (Banco Existente)

## 📌 CENÁRIO

Você já tem:
- ✅ Banco PostgreSQL no Supabase funcionando
- ✅ Tabelas criadas (`employers`, `candidates`, `interviews`)
- ✅ API testada localmente

**Objetivo:** Deploy no Railway usando o banco Supabase existente

---

## ⚡ DEPLOY RÁPIDO (5 MINUTOS)

### 1️⃣ Push para GitHub

```bash
git add .
git commit -m "Configurar Railway com Supabase"
git push origin main
```

### 2️⃣ Criar projeto no Railway

1. Acesse https://railway.app
2. **"New Project"** → **"Deploy from GitHub repo"**
3. Selecione: `GLOBAL_SOLUTION_CSHARP`
4. Aguarde build (vai falhar por falta de banco - normal!)

### 3️⃣ Configurar variável DATABASE_URL

1. No Railway, clique no serviço da **API**
2. Aba **"Variables"**
3. Clique **"+ New Variable"**
4. Configure:

**Nome:** `DATABASE_URL`

**Valor:** (escolha um dos seus bancos)

**Opção A - Supabase Pooler (Produção - RECOMENDADO):**
```
Host=aws-1-us-east-1.pooler.supabase.com;Port=5432;Database=postgres;Username=postgres.meawpenzwaxszxhweehh;Password=ju153074;Ssl Mode=Require;
```

**Opção B - Supabase Direto (Alternativa):**
```
Host=db.tisnibdevnjdynbcjyqm.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=ju153074;Ssl Mode=Require;Trust Server Certificate=true;Pooling=true;Minimum Pool Size=1;Maximum Pool Size=20;Connection Idle Lifetime=300;
```

> 💡 **Recomendo Opção A** - Pooler é otimizado para produção!

5. Clique em **"Add"**
6. Railway vai fazer **redeploy automático**

### 4️⃣ Gerar domínio público

1. Ainda no serviço da API
2. Aba **"Settings"**
3. Seção **"Networking"** → **"Generate Domain"**
4. Copie a URL gerada (ex: `entrevistas-api.up.railway.app`)

### 5️⃣ Testar API

Abra no navegador:

```
https://sua-url.up.railway.app/swagger
```

✅ Se carregar o Swagger = **SUCESSO!**

---

## 🎯 VERIFICAÇÃO COMPLETA

### Testar endpoints no Swagger

1. **GET** `/api/v1/empregadores` - Lista empregadores
2. **GET** `/api/v1/candidatos` - Lista candidatos
3. **GET** `/api/v1/entrevistas` - Lista entrevistas
4. **GET** `/api/v1/entrevistas/dashboard` - Estatísticas

Se retornar dados do Supabase = **PERFEITO!**

---

## 📊 VERIFICAR LOGS

1. Railway → Serviço API → **"Deployments"**
2. Clique no último deploy
3. Procure por:

✅ **Mensagens de sucesso:**
```
🔗 Using connection: DATABASE_URL (Cloud)
✅ Conexão com banco de dados estabelecida com sucesso!
🌍 API rodando em ambiente de PRODUÇÃO
Now listening on: http://0.0.0.0:8080
```

---

## 🔧 SE DER ERRO

### Erro: "Connection string not found"

**Causa:** Variável `DATABASE_URL` não foi adicionada

**Solução:**
1. Verifique se você adicionou a variável
2. Nome deve ser exatamente: `DATABASE_URL` (maiúsculas)
3. Valor deve ter todo o connection string

### Erro: "password authentication failed"

**Causa:** Senha incorreta no connection string

**Solução:**
1. Vá no Supabase: Settings → Database → Connection String
2. Copie o connection string atualizado
3. Substitua `[YOUR-PASSWORD]` pela senha real: `ju153074`
4. Atualize variável no Railway

### Erro: SSL/Certificate

**Causa:** Configuração SSL incorreta

**Solução:** Use o connection string da **Opção A** (Pooler) que tem `Ssl Mode=Require;`

---

## 💰 CUSTO

**Railway Free Tier:**
- ✅ $5 USD/mês grátis
- ✅ 500 horas de execução
- ✅ Seu projeto: ~$1-2 USD/mês

**Supabase Free Tier:**
- ✅ Continua grátis
- ✅ Sem mudanças

**Total:** $0 (dentro dos créditos gratuitos)

---

## 📝 ATUALIZAR README

Após deploy bem-sucedido, adicione no README:

```markdown
## 🌐 API em Produção

**URL da API:** https://sua-url.up.railway.app

**Swagger (Documentação):** https://sua-url.up.railway.app/swagger

**Tecnologias:**
- Backend: .NET 9.0 + ASP.NET Core
- Banco: PostgreSQL (Supabase)
- Deploy: Railway.app
- ORM: Entity Framework Core 9.0

**Credenciais (Somente para avaliadores):**
- Banco: PostgreSQL no Supabase (gerenciado)
- URL da API: Acesso público via link acima
```

---

## 🎥 PARA O VÍDEO

Mencione:

> "A API foi publicada no Railway.app utilizando banco de dados PostgreSQL hospedado no Supabase. O deploy é automatizado através de Dockerfile e conexão configurada via variável de ambiente DATABASE_URL. A aplicação roda 24/7 sem hibernação, diferente de plataformas como Render free tier."

**Mostre:**
1. Dashboard do Railway (projeto rodando)
2. Swagger funcionando na URL pública
3. Testar alguns endpoints ao vivo

---

## ✅ CHECKLIST FINAL

- [ ] Código no GitHub atualizado
- [ ] Projeto criado no Railway
- [ ] Variável `DATABASE_URL` adicionada (Supabase)
- [ ] Deploy bem-sucedido (status verde)
- [ ] Domínio público gerado
- [ ] Swagger acessível online
- [ ] Endpoints testados e funcionando
- [ ] URL adicionada no README
- [ ] URL incluída na entrega do Teams

---

## 🎉 PRONTO!

Seu projeto agora está:
- ✅ Publicado na nuvem (Railway)
- ✅ Usando banco de dados em produção (Supabase)
- ✅ Disponível 24/7 via HTTPS
- ✅ Pronto para demonstração
- ✅ Pronto para entrega na FIAP

**Tempo total:** 5-10 minutos
**Custo:** $0 (dentro do free tier)

---

## 📞 Precisa de ajuda?

**Railway:**
- Docs: https://docs.railway.app
- Discord: https://discord.gg/railway

**Supabase:**
- Docs: https://supabase.com/docs
- Discord: https://discord.supabase.com
