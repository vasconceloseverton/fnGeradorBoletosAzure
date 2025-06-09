
# 📦 Azure Function - Gerador de Código de Barras

Esta Azure Function foi desenvolvida para processar requisições HTTP contendo dados de pagamento, gerar um código de barras e publicar o resultado em uma fila do Azure Service Bus.

## 🚀 Funcionalidade

- Recebe uma requisição `POST` contendo os seguintes dados:
  - `valor`: valor numérico do boleto.
  - `dataVencimento`: data de vencimento no formato `yyyy-MM-dd`.
  - `barcodeData`: dados adicionais necessários para gerar o código de barras.
- Valida os dados recebidos.
- Gera um objeto com:
  - `barcode`
  - `valorOriginal`
  - `DataVencimento`
  - `ImageBase64`
- Envia esse objeto para a fila `gerador-codigo-barras` no Azure Service Bus.

---

## 🔧 Configuração

### Variáveis de Ambiente

Certifique-se de definir a seguinte variável no ambiente da Azure Function:

| Variável                    | Descrição                                   |
|----------------------------|---------------------------------------------|
| `ServiceBusConnectionString` | String de conexão do Azure Service Bus.     |

---

## 📥 Exemplo de Requisição

```http
POST /api/barcode-generator HTTP/1.1
Content-Type: application/json

{
  "valor": "123.45",
  "dataVencimento": "2025-07-15",
  "barcodeData": "dados-para-geracao"
}
```

---

## ✅ Respostas Esperadas

### Sucesso

```json
"Welcome to Azure Functions!"
```

### Erros Comuns

| Código HTTP | Mensagem                                   | Causa                                           |
|-------------|--------------------------------------------|------------------------------------------------|
| `400`       | "Invalid input data."                      | Algum campo obrigatório está ausente ou vazio. |
| `400`       | "Invalid date format for dataVencimento."  | Formato da data incorreto.                     |
| `400`       | "Invalid value format for valor."          | Valor não numérico ou fora do intervalo.       |
| `500`       | Erro interno.                              | Exceção inesperada durante o processamento.    |

---

## 📤 Envio ao Azure Service Bus

Após a validação, o seguinte payload é enviado para a fila `gerador-codigo-barras`:

```json
{
  "barcode": "00012323123258888200203123734",
  "valorOriginal": 123.45,
  "DataVencimento": "2025-07-15",
  "ImageBase64": "ImagemBase64"
}
```

---

## 🛠️ Tecnologias Utilizadas

- C#
- Azure Functions
- Azure Service Bus
- Newtonsoft.Json
- Azure.Messaging.ServiceBus

---

## 📁 Estrutura da Solução

```
fnGeradorBoletosAzure/
│
├── GeradorCodigoBarras.cs       # Função principal
├── host.json                    # Configuração da Azure Function
├── local.settings.json          # Configurações locais (não subir para o Git)
└── README.md                    # Este documento
```

---

## 📝 Observações

- Este projeto é um exemplo e ainda possui valores estáticos para o código de barras e imagem. A geração real deve ser implementada conforme necessário.
- A função retorna apenas uma string de boas-vindas, mas o resultado real é enviado pela fila.
