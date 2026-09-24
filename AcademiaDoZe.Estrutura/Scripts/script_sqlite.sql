CREATE TABLE IF NOT EXISTS tb_logradouro (
    id_logradouro INTEGER PRIMARY KEY AUTOINCREMENT,
    cep VARCHAR(8) NOT NULL UNIQUE,
    nome VARCHAR(150) NOT NULL,
    bairro VARCHAR(100) NOT NULL,
    cidade VARCHAR(100) NOT NULL,
    estado CHAR(2) NOT NULL,
    pais VARCHAR(50) NOT NULL DEFAULT 'Brasil'
);

CREATE INDEX IF NOT EXISTS ix_tb_logradouro_cep
ON tb_logradouro(cep);

CREATE INDEX IF NOT EXISTS ix_tb_logradouro_cidade
ON tb_logradouro(cidade);


CREATE TABLE IF NOT EXISTS tb_aluno (
    id_aluno INTEGER PRIMARY KEY AUTOINCREMENT,
    cpf VARCHAR(11) NOT NULL UNIQUE,
    nome VARCHAR(150) NOT NULL,
    nascimento DATE NOT NULL,
    telefone VARCHAR(15) NOT NULL,
    email VARCHAR(150) NOT NULL,
    logradouro_id INTEGER NOT NULL,
    numero VARCHAR(20) NOT NULL,
    complemento VARCHAR(100),
    senha VARCHAR(255) NOT NULL,
    foto BLOB,

    FOREIGN KEY (logradouro_id)
        REFERENCES tb_logradouro(id_logradouro)
);


CREATE INDEX IF NOT EXISTS ix_tb_aluno_cpf
ON tb_aluno(cpf);


CREATE TABLE IF NOT EXISTS tb_colaborador (
    id_colaborador INTEGER PRIMARY KEY AUTOINCREMENT,
    cpf VARCHAR(11) NOT NULL UNIQUE,
    nome VARCHAR(150) NOT NULL,
    nascimento DATE NOT NULL,
    telefone VARCHAR(15) NOT NULL,
    email VARCHAR(150) NOT NULL,
    logradouro_id INTEGER NOT NULL,
    numero VARCHAR(20) NOT NULL,
    complemento VARCHAR(100),
    senha VARCHAR(255) NOT NULL,
    foto BLOB,
    admissao DATE NOT NULL,
    tipo INTEGER NOT NULL,
    vinculo INTEGER NOT NULL,

    FOREIGN KEY (logradouro_id)
        REFERENCES tb_logradouro(id_logradouro)
);


CREATE INDEX IF NOT EXISTS ix_tb_colaborador_cpf
ON tb_colaborador(cpf);


CREATE TABLE IF NOT EXISTS tb_matricula (
    id_matricula INTEGER PRIMARY KEY AUTOINCREMENT,
    aluno_id INTEGER NOT NULL,
    plano INTEGER NOT NULL,
    data_inicio DATE NOT NULL,
    data_fim DATE NOT NULL,
    objetivo VARCHAR(500) NOT NULL,
    restricao_medica INTEGER NOT NULL DEFAULT 0,
    obs_restricao VARCHAR(500),
    laudo_medico BLOB,

    FOREIGN KEY (aluno_id)
        REFERENCES tb_aluno(id_aluno)
        ON DELETE CASCADE
);


CREATE INDEX IF NOT EXISTS ix_tb_matricula_aluno_id
ON tb_matricula(aluno_id);

CREATE INDEX IF NOT EXISTS ix_tb_matricula_data_fim
ON tb_matricula(data_fim);


CREATE TABLE IF NOT EXISTS tb_acesso (
    id_acesso INTEGER PRIMARY KEY AUTOINCREMENT,
    pessoa_tipo INTEGER NOT NULL,
    pessoa_id INTEGER NOT NULL,
    data_hora DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);


CREATE INDEX IF NOT EXISTS ix_tb_acesso_pessoa
ON tb_acesso(pessoa_tipo, pessoa_id);

CREATE INDEX IF NOT EXISTS ix_tb_acesso_data_hora
ON tb_acesso(data_hora);