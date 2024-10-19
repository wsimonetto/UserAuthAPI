Feature: Recuperação de Senha

Scenario: Solicitar recuperação de senha
    Given que o usuário fornece um e-mail "user@example.com"
    When o usuário solicita a recuperação de senha
    Then o sistema deve enviar um e-mail com instruções de recuperação
    And o e-mail deve ser enviado para "user@example.com"