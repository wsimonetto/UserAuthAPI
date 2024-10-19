Feature: Cadastro de Usuário

Scenario: Cadastro com sucesso
    Given que o usuário fornece um cadastro com e-mail "novo_user@example.com" e senha "senhaSegura123"
    When o usuário se cadastra
    Then o cadastro deve ser bem-sucedido
    And o usuário deve receber uma confirmação de cadastro