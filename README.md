
# 📦 Azure Function - Gerador de Código de Barras

## Visão Geral

Este projeto implementa uma Azure Function responsável por processar dados de pagamento enviados via requisição HTTP, gerar informações simuladas de código de barras e enviar essas informações para uma fila no Azure Service Bus.

Essa função é ideal para integrar processos de geração e controle de boletos ou qualquer tipo de cobrança que utilize códigos de barras, podendo ser acoplada a sistemas financeiros, ERPs, aplicativos ou serviços de backoffice.

---

## 🚀 Funcionalidades

- ✅ Recebe requisições HTTP no método `POST` com conteúdo JSON.
- ✅ Valida os dados obrigatórios da requisição: `valor`, `dataVencimento` e `barcodeData`.
- ✅ Valida formato da data e valor numérico com limite de casas decimais.
- ✅ Gera resposta simulada com:
  - Código de barras (`barcode`)
  - Valor original (`valorOriginal`)
  - Data de vencimento (`DataVencimento`)
  - Representação da imagem em base64 (`ImageBase64`)
- ✅ Envia o objeto resultante para uma fila do Azure Service Bus (`gerador-codigo-barras`).

---

## 🛠️ Tecnologias Utilizadas

- [Azure Functions](https://learn.microsoft.com/azure/azure-functions/)
- [C#](https://learn.microsoft.com/dotnet/csharp/)
- [Azure.Messaging.ServiceBus](https://learn.microsoft.com/dotnet/api/overview/azure/messaging.servicebus-readme)
- [Newtonsoft.Json](https://www.newtonsoft.com/json)
- Azure SDKs e SDK de Logging

---

## 📦 Requisitos

- Conta no Azure com Service Bus provisionado
- Azure Functions configurado no ambiente desejado (consumo, premium ou dedicado)
- .NET 6 ou superior

---

## 🧩 Instalação e Execução Local

1. Clone este repositório:
```bash
git clone https://github.com/vasconceloseverton/fnGeradorBoletosAzure.git
cd seu-repositorio
```

2. Configure o arquivo `local.settings.json`:

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "ServiceBusConnectionString": "<sua-connection-string>"
  }
}
```

3. Execute localmente com o Core Tools:
```bash
func start
```

---

## 🔧 Configurações de Ambiente

| Variável                      | Descrição                                                                 |
|------------------------------|---------------------------------------------------------------------------|
| `ServiceBusConnectionString` | String de conexão com o Azure Service Bus (SharedAccessKey ou Managed Id) |

---

## 📥 Exemplo de Requisição HTTP

**Endpoint:** `/api/barcode-generator`  
**Método:** `POST`  
**Content-Type:** `application/json`

```json
{
  "valor": "123.45",
  "dataVencimento": "2025-07-15",
  "barcodeData": "dados-para-geracao"
}
```

---

## ✅ Respostas Esperadas

### ✅ Sucesso (200 OK)

```text
"Welcome to Azure Functions!"
```

### ❌ Erros Comuns

| Código | Motivo                                              | Solução                                   |
|--------|------------------------------------------------------|-------------------------------------------|
| 400    | "Invalid input data."                                | Verifique se todos os campos estão ok     |
| 400    | "Invalid date format for dataVencimento."            | Use formato `yyyy-MM-dd`                  |
| 400    | "Invalid value format for valor."                    | Use um número decimal válido              |
| 500    | "An error occurred while generating the barcode."    | Verifique os logs para mais informações   |

---

## 📤 Exemplo de Payload Enviado para a Fila do Service Bus

```json
{
  "barcode": "00012323123258888200203123734",
  "valorOriginal": 123.45,
  "DataVencimento": "2025-07-15",
  "ImageBase64": "ImagemBase64"
}
```


## 📝 Observações

- O código de barras e imagem são gerados de forma simulada (mock).
- O envio real do boleto, PDF ou validações adicionais não fazem parte desta função.
- Pode ser integrado facilmente com filas, bancos de dados e APIs externas para uma solução completa.

---

## 📬 Contato

Para dúvidas ou sugestões, entre em contato com o mantenedor do projeto ou abra uma issue.

---

## 🧾 Licença

Este projeto está licenciado sob a [MIT License](LICENSE).
