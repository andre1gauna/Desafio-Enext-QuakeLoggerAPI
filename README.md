# QuakeLogger

Este arquivo Readme visa explicar o funcionamento da aplicação, assim como um breve detalhamento de seus métodos internos para 
que outro desenvolvedor possa compreender e dar prosseguimento, se assim desejar, ao projeto.

## O que faz?

Esta aplicação é um Parser de logs de partidas do Quake III Arena. Ela irá busca um arquivo com o nome "raw" na raiz do projeto e realizará operações sobre ele para poder exibir tanto no console quanto via API Rest as informações das partidas como: quem matou quem, quantidade de mortes por jogador, quantidade total de mortes por partida e metódos de morte com suas respectivas quantidades por partida.
Obs: para cada morte que um jogador sofre, ele perde uma kill.

## Funcionamento

Para rodar a aplicação, abra o arquivo QuakeLoggerAPI.sln no Visual Studio e execute-a.

A aplicação opera da seguinte forma: A classe principal que realiza o parser do Log de partidas - chamada Parser - recebe em 
seu construtor uma string com o caminho do arquivo .txt que será trabalhado, e o método Reader() desta classe é quem dá início
à leitura do arquivo e processa todas as suas informações relevantes, alocando-as em um banco de dados em memória através de
classes de repositório.
Os dados do banco em memória são exibidos no console logo ao inicializar a aplicação, assim como também serão acessíveis através
de uma API.

Imediatamente ao ser inicializada, a aplicação realiza a análise e processamento supracitados e abre uma janela do navegador
padrão com uma página do Swagger, um framework utilizado para a documentação de APIs REST. Nesta página, é apresentado o método
GetById, onde é possível inserir o Id da partida desejada e executar a busca pela API. Dessa forma, será retornado um JSON
com as informações da partida, seus jogadores, a quantidade de kills por jogador, quantidade total de Kills e os Kill Methods
com suas respectivas quantidades de kills por método. Pelo arquivo fornecido, foram alocadas 21 partidas, sendo este o valor máximo
para ser inserido no campo do método GetById. Caso seja excedido o valor, será retornado erro 404 - Not Found.
Concomintate à abertura da página no Swagger, o console da aplicação exibirá as mesmas informações, porém sem o detalhamento
dos Kill Methods. Esta exibição é realizada pelo método Print() da classe ReportPrinter.

### Métodos da classe principal - Parser
	
        Todos os métodos são privados, com exceção dos que estiverem descritos como público.

+ Reader()
	Público - Realiza a leitura linha a linha do arquivo, chamando o método LineChecker() para que este analise
	a linha atual

+ LineChecker()
	A depender da informação em formato string[] que receber, este método pode criar uma partida, adicionar jogadores, adicionar
	Kill Methods e fechar uma partida.

+ CreateGame()
	Cria uma partida no banco de dados em memória caso a linha atual contenha a string "InitGame:".

+ Closegame()
	Finaliza a partida recém criada no banco de dados tualizando suas informações caso a linha atual contenha "ShutdownGame:"

+ FindKiller()
	Encontra o killer atual com base em seu nome.

+ FindKilled()
	Encontra o killed atual com base em seu nome.	
	
+ GetKillMethod()
	Encontra o KillMethod atual com base em seu nome
	
+ AddKillMethod()
	Adiciona o KillMethod atual no banco em memória, ou então incrementa sseu contador caso ja exista no banco em memória.
	
+ AddPlayer()
    Adiciona um Player à partida caso este não exista no banco.

+ HasGame()
	Confere se o Player está inserido no game atual.
	
+ AddPlayerTogame()
	Adiciona o Player à partida atual caso ele não esteja presente.
	
+ AddKill()
	Adiciona uma Kill ao Player caso ele seja um killer, ou decrementa uma kill caso ele tenha sido morto (killed). A quantidade de kills nunca é menor do que 0.

+ ReverseString()
	Método auxiliar para reverter uma string. Necessário para corrigir o nome do killer.
	
+ GetGameById()
	Público - Retorna um Game respectivo ao Id inserido. Utilizado nos testes
	
+ GetPlayersByGameId()
	Público - Retorna uma lista de Players de acordo com Id do Game. Utilizado nos testes.

+ GetKillMethodsByGameId()
	Público - Retorna uma lista de KillMethods de acordo com Id do Game. utilizado nos testes
	
### Testes
Dentro da solução, há o projeto QuaeLogger.Tests, onde é possível testar algumas das funcionalidades principais da aplicação. Não é possível testar os
metodos privados. Para executar os testes, execute-os pelo gerenciador de testes do Visual Studio. O contexto de banco de dados utilizado nos textes será gerado em memória, e utilzará um arquivo na raiz do projeto chamado testRaw para fazer o parser.

