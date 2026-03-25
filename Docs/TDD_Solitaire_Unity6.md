# Technical Design Document (TDD)

## Projeto: Solitaire -- Unity 6

### 1. Stack Tecnológica

Engine: Unity 6\
Linguagem: C#\
Renderização: 2D (SpriteRenderer)\
Controle de versão: Git

### 2. Arquitetura Geral

Arquitetura modular com separação entre:

-   GameManager → controla fluxo do jogo
-   DeckManager → cria e gerencia o baralho
-   RulesValidator → valida regras de movimento
-   Pile / CardStackView → pilhas de cartas
-   Card / CardView → modelo e representação visual

### 3. Estrutura de Pastas

Assets/ Scripts/ Core/ Cards/ Piles/ Managers/ UI/ Prefabs/ Scenes/

### 4. Fluxo de Inicialização

1.  GameManager inicia o jogo
2.  DeckManager cria e embaralha o baralho
3.  Cartas são distribuídas no Tableau
4.  Restante vai para Stock

### 5. Principais Classes

GameManager - controla estados do jogo - inicia partidas - verifica
condição de vitória

DeckManager - cria baralho - embaralha cartas - fornece cartas para o
jogo

RulesValidator - valida movimentos - aplica regras do Klondike

### 6. Sistema de Layout

O layout da mesa é controlado por um script BoardLayout que calcula
posições com base no tamanho da câmera ortográfica.

### 7. Persistência

Inicialmente utilizando PlayerPrefs para estatísticas básicas.

### 8. Testes

-   Testes unitários para RulesValidator
-   Testes funcionais para fluxo de jogo
