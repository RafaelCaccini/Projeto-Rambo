# Documentação do Script: TanqueSentinela

**Local do arquivo:** `Assets/Documentação/TanqueDocumentacao.md`

## Visão Geral
Este script (`TanqueSentinela.cs`) controla um tanque sentinela que detecta o jogador, dispara projéteis e executa uma animação de recuo no cano após cada disparo.  
Deve ser anexado a um objeto no Unity que represente o tanque.

---

## Principais Funcionalidades

### 1. Detecção do Jogador
- Procura um jogador na cena pelo nome (`nomeDoJogador`).
- Usa um raio de detecção (`raioDeteccao`) para identificar quando o jogador está próximo.

### 2. Disparo de Projéteis
- Dispara um projétil a partir de um ponto de disparo (`pontoDeDisparo`) em intervalos definidos (`intervaloTiros`).
- O projétil recebe uma força (`forcaTiro`) e é destruído automaticamente após 2 segundos.
- É marcado com a tag `"Danger"`.

### 3. Animação de Recuo
- Após cada disparo, o cano executa um movimento de recuo e volta ao estado original.
- Configurado pelos parâmetros: `fatorRecuo`, `duracaoRecuo` e `duracaoVolta`.

### 4. Visualização no Editor
- Um gizmo vermelho mostra o raio de detecção do tanque quando o objeto está selecionado.

---

## Como Utilizar
1. Crie um objeto na cena para representar o tanque.
2. Anexe o script **TanqueSentinela.cs** a este objeto.
3. Configure os parâmetros:
   - **nomeDoJogador**: Nome do GameObject que representa o jogador.
   - **projetilPrefab**: Prefab do projétil a ser disparado.
   - **pontoDeDisparo**: Transform que indica a posição de saída do disparo.
   - **canoTanque**: Transform do cano para aplicar o recuo.

---

## Fluxo de Funcionamento
1. O script procura o jogador na cena (`Start()`).
2. No `Update()`, verifica se o jogador está dentro do raio de detecção.
3. Se estiver, dispara projéteis conforme o intervalo configurado.
4. Cada disparo aciona a animação de recuo do cano.

---

## Observações
- O projétil deve conter um `Rigidbody2D` para receber a força do disparo.
- O tanque só dispara se o jogador for encontrado e estiver dentro do raio de detecção.

---

## Localização no Projeto
- **Script:** `Assets/Scripts/TanqueSentinela.cs`
- **Documentação:** `Assets/Documentação/TanqueDocumentacao.md`

---

## Direitos Autorais e Licença

© 2025 Lolota Studios. **Todos os direitos reservados.**

Este software, incluindo seus scripts, dependências e documentação, é propriedade exclusiva da **Lolota Studios**.  
Não é permitido copiar, modificar, distribuir, sublicenciar, comercializar ou utilizar este código ou partes dele, exceto mediante autorização expressa e por escrito da Lolota Studios.

Qualquer uso não autorizado é estritamente proibido e sujeito a medidas legais.

**Programador responsável:** Rafael Lins. (Discord: lnsxdrk. Instagram: rafaellins08)
**Data de criação do código:** 01 de setembro de 2025, 22:00
**Data de criação da documentação** 02 de setembro de 2025, 20:40
