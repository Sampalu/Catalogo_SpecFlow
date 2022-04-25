Funcionalidade: Categorias Web Api

Cenário: Obter Categorias
	Dado eu sou um httpClient
	E o repositório tem dados
	Quando eu fizer uma requisição GET em 'categorias'
	Então o código de resposta deve ser '200'
	E o json deve ser lista de 'categorias.json'

Cenário: Obter Categoria
	Dado eu sou um httpClient
	E o repositório tem dados
	Quando eu fizer uma requisição GET com id '1' em 'categorias'
	Então o código de resposta deve ser '200'
	E o json deve ser 'categoria.json'

Cenário: Adicionar Categoria
	Dado eu sou um httpClient
	Quando eu fizer uma requisição POST com 'categoria3.json' em 'categorias'
	Então o código de resposta deve ser '201'
	E o location no header é 'https://localhost:5001/api/v1/Categorias/4'
	E o json deve ser 'categoria3.json'

Cenário: Atualizar Categoria
	Dado eu sou um httpClient
	E o repositório tem dados
	Quando eu fizer uma requisição PUT com 'categoria2.json' com id '1' em 'categorias'
	Então o código de resposta deve ser '200'
	E o json deve ser 'categoria2.json'
	
Cenário: Remover Categoria
	Dado eu sou um httpClient
	E o repositório tem dados
	Quando eu fizer uma requisição DELETE com id '1' em 'categorias'
	Então o código de resposta deve ser '200'