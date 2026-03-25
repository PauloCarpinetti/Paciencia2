# Game Design Document (GDD)

## Projeto: Solitaire (Klondike) -- Unity 6

### 1. Visão Geral

Solitaire é um jogo de cartas single‑player baseado na variação clássica
Klondike. O objetivo é organizar todas as cartas do baralho em quatro
pilhas de fundação, separadas por naipe e em ordem crescente.

### 2. Objetivos do Projeto

-   Recriar a experiência clássica de Solitaire
-   Interface limpa e intuitiva
-   Código modular e escalável
-   Base sólida para futuras expansões (temas, estatísticas, dicas)

### 3. Público‑Alvo

-   Jogadores casuais
-   Idade 12+
-   Sessões de jogo curtas

### 4. Plataforma

-   PC
-   WebGL
-   Mobile (Android / iOS)

### 5. Mecânicas Principais

Baralho padrão de 52 cartas.

Tipos de pilha: - Stock - Waste - Tableau (7 colunas) - Foundations (4
pilhas)

Regras principais: - Tableau organizado em ordem decrescente - Cores
alternadas - Foundations organizadas por naipe em ordem crescente -
Apenas Reis podem ocupar espaços vazios

### 6. Interface do Usuário

Elementos principais: - Botão Novo Jogo - Botão Desfazer -
Temporizador - Pontuação - Menu de pausa

### 7. Condição de Vitória

O jogo é vencido quando todas as cartas são movidas para as Foundations.

### 8. Roadmap Inicial

Sprint 0: Planejamento e setup\
Sprint 1: Fundação do sistema de cartas\
Sprint 2: Core gameplay\
Sprint 3: UI e UX\
Sprint 4: Polimento e testes
