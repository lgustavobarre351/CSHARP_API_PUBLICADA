# ✅ CHECKLIST - Deploy Railway.app

## 📋 **PASSO A PASSO COMPLETO**

### **PREPARAÇÃO** ✅ 
- [x] Código no GitHub
- [x] Dockerfile configurado
- [x] railway.json criado
- [x] Variáveis de produção configuradas

---

### **RAILWAY SETUP** 
- [ ] 1. Acessar https://railway.app
- [ ] 2. Login com GitHub
- [ ] 3. "New Project" → "Deploy from GitHub repo"
- [ ] 4. Selecionar repositório `SPRINT4_CSHARP_API`
- [ ] 5. Clicar "Deploy Now"

---

### **POSTGRESQL SETUP**
- [ ] 6. "New Service" → "Database" → "Add PostgreSQL"
- [ ] 7. Aguardar criação do banco (1-2 min)

---

### **VARIÁVEIS DE AMBIENTE**
- [ ] 8. Clicar no serviço da API
- [ ] 9. Aba "Variables"
- [ ] 10. Adicionar: `ASPNETCORE_ENVIRONMENT=Production`
- [ ] ✅ `PORT` e `DefaultConnection` são automáticos!

---

### **DEPLOY & TESTE**
- [ ] 11. Aguardar build (3-5 min)
- [ ] 12. Copiar URL gerada
- [ ] 13. Testar: `https://sua-url/swagger`
- [ ] 14. Testar endpoints da API

---

## 🎯 **LINKS IMPORTANTES**

- **Railway Dashboard:** https://railway.app/dashboard
- **Documentação:** https://docs.railway.app
- **Suporte:** https://help.railway.app

---

## 🆘 **TROUBLESHOOTING**

### **Build falhou?**
- Verifique se o Dockerfile está na pasta `Investimentos/`
- Confirme se o `railway.json` está na raiz do projeto

### **Erro de conexão com banco?**
- Verifique se PostgreSQL foi adicionado
- A variável `DefaultConnection` deve aparecer automaticamente

### **API não responde?**
- Verifique logs na aba "Deployments"
- Confirme se `ASPNETCORE_ENVIRONMENT=Production` está definida

### **Swagger não carrega?**
- Acesse: `https://sua-url/swagger` (com /swagger no final)
- Verifique se a API está respondendo na raiz: `https://sua-url/`

---

## 🎉 **APÓS DEPLOY SUCESSO**

Sua API terá:
- ✅ **URL pública** permanente
- ✅ **SSL automático** (HTTPS)
- ✅ **PostgreSQL** configurado
- ✅ **Swagger UI** ativo
- ✅ **Zero hibernação** - online 24/7!
- ✅ **Deploys automáticos** a cada push no GitHub

**CUSTO:** $0 - $2 USD/mês (dentro dos $5 gratuitos)