# Custom ASP.NET Style Server 🚀

Um servidor HTTP de alto desempenho desenvolvido do zero em **C#**, inspirado na arquitetura do **ASP.NET Core**. Este projeto foca em demonstrar o ciclo de vida completo de uma requisição, desde a captura do socket TCP até o roteamento dinâmico para controladores.

## 📌 Sobre o Projeto

O objetivo deste projeto é entender "o que acontece por baixo do capô" de frameworks modernos. Em vez de usar abstrações prontas como o Kestrel, este servidor gerencia conexões, interpreta cabeçalhos HTTP e utiliza **Reflection** para mapear rotas dinamicamente.

### ✨ Funcionalidades Principais

* **HTTP Parsing:** Processamento manual de Verbos (GET, POST, PUT, DELETE), Headers e Body.
* **Dynamic Routing:** Sistema que identifica automaticamente classes `Controller` e métodos via Reflection.
* **Controller Base:** Abstração para criação de endpoints de forma intuitiva.
* **JSON Serialization:** Suporte nativo para retorno de objetos em formato JSON.
* **Thread Management:** Gerenciamento de múltiplas conexões simultâneas.

## 🛠️ Tecnologias e Conceitos

* **Linguagem:** C# (.NET )
* **Protocolo:** TCP/IP & HTTP/1.1

## 💻 Exemplo de Implementação

```csharp
public class UserController : Controller
{
    [HttpGet("user/users")]
    public ActionResult GetUser()
    {
        return Ok(new { Id = 1, Name = "Kaiky", Role = "Developer" });
    }
}
