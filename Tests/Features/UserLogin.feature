Feature: Login de Usuário
  Como um usuário
  Quero fazer login no sistema
  Para acessar minhas informações pessoais

Scenario: Login com credenciais válidas
  Given que existe um usuário com email "user@example.com" e senha "senha123"
  When o usuário tenta fazer login com email "user@example.com" e senha "senha123"
  Then o login deve ser bem-sucedido