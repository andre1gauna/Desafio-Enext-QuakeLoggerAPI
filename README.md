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
from pdf2image import convert_from_path 
import numpy as np 
from PIL import Image 
import cv2 
import pytesseract 
import pyodbc
from fuzzywuzzy import fuzz

# str1 = "Ola, como vai?"

# str2 = "O lá c0m o aãai*"
# ratio = fuzz.ratio (str1.lower(), str2.lower())
# print(ratio)

# server = 'sqndsc001'
# db1 = 'DBTB420'
# uname = 'gaunaan'
# pswd = '987123'
# conexao = pyodbc.connect(driver='{​​SQL Server}​​',
#     server = server,
#     database = db1,
#     uid= uname,
#     pwd = pswd)


poppler_path = r'poppler-0.68.0\bin' 
pytesseract.pytesseract.tesseract_cmd= r'Tesseract-OCR\tesseract.exe'


def Leitura(prefixo,imagens):
    global contadorArea
    global Lista_Areas
    global valorMinimoAreaRelativa
    global Lista_campos_Final
    global Lista_width_Final
    global Lista_height_Final
    global criarUmTxtPorImagem
    contadorImagem=0
    i=0     
    
    for imagem in imagens:
        
        contadorImagem += 1
        paginaAtual = prefixo+ str(contadorImagem) + '.jpg' 
        imagem.save(paginaAtual, "JPEG") 
        img = cv2.imread(prefixo+ str(contadorImagem) + '.jpg', 0) 
        img2 = cv2.imread(prefixo+ str(contadorImagem) + '.jpg') 
        _, thrash = cv2.threshold(img,200,255, cv2.THRESH_BINARY) 
        contornos, hierarquias = cv2.findContours(thrash, cv2.RETR_LIST, cv2.CHAIN_APPROX_SIMPLE)
        Altura = img.shape[0] 
        Largura = img.shape[1] 
        AreaTotal = Altura*Largura 
        x_anterior=0 
        y_anterior=0 
        h_anterior = 0 
        w_anterior = 0 
        areaRetangulo_Anterior = 0      
        j = 0
        a = 0
        Lista_campos_Atual = []
        Lista_width_Atual =[] 
        Lista_height_Atual = []              
        contadorHierarq = 0
        for contorno in contornos:
             
            contornoCorrigido = cv2.approxPolyDP(contorno, 0.015* cv2.arcLength(contorno,True), True)
            # x,y,w,h = cv2.boundingRect(contornoCorrigido)
            # if (x in range(315,335) and y in range(1405,1425)): 
            #         a = len(contornoCorrigido)
            #         cv2.drawContours(img2, [contorno], 0, (200,69,30), 3)
            #         cv2.imwrite('pacampos'+str(contador)+'.jpg',img2)
            #         b=20 
            if len(contornoCorrigido) == 4: 
                x,y,w,h = cv2.boundingRect(contornoCorrigido) 
                if y==0: 
                    y=0.01 
                if x==0: 
                    x=0.01 
                if (x in range(185,200) and y in range(290,305)): 
                    ponto = 20 
                areaRetangulo = w*h 
                areaRelativa = AreaTotal/areaRetangulo 
                razaoX = x_anterior/x 
                razaoY = y_anterior/y 
                relacaoPontoX = 0.85<=(razaoX)<=1.15 
                relacaoPontoY = 0.85<=(razaoY)<=1.15
                if (areaRelativa < 3000 and areaRelativa > 5):                
                    if(relacaoPontoX is True and relacaoPontoY is True):                                                                
                        if(areaRetangulo<areaRetangulo_Anterior):
                            campo = thrash[y:(y+h), x:(x+w)]
                            if (areaRelativa > valorMinimoAreaRelativa): 
                                cv2.imwrite('Salvos/'+ prefixo + str(contadorImagem) +'campo'+str(j-1)+'.png', campo)
                                Lista_campos_Atual[j-1]=Image.open('Salvos/'+ prefixo + str(contadorImagem) +'campo'+str(j-1)+'.png')
                                Lista_height_Atual[j-1]=h
                                Lista_width_Atual[j-1]=w
                                x_anterior = 0.1
                                y_anterior = 0.1
                                cv2.drawContours(img2, [contorno], 0, (200,69,30), 3)
                            else:
                                cv2.imwrite('Salvos/'+ prefixo + str(contadorImagem) + 'Area' + str(a-1)+'.png', campo)
                                Lista_Areas[a-1]=Image.open('Salvos/'+ prefixo + str(contadorImagem) + 'Area' + str(a-1)+'.png')                                
                                x_anterior = 0.1
                                y_anterior = 0.1
                                cv2.drawContours(img2, [contorno], 0, (200,69,30), 3)

                    else:
                        if (areaRelativa > valorMinimoAreaRelativa): 
                            cv2.drawContours(thrash, [contorno], 0, (255,255,255), 11)                           
                            campo = thrash[y:(y+h), x:(x+w)]                                               
                            cv2.imwrite('Salvos/'+ prefixo  +str(contadorImagem) +'campo'+str(j)+'.png',campo) 
                            Lista_campos_Atual.append(Image.open('Salvos/'+ prefixo  +str(contadorImagem) +'campo'+str(j)+'.png')) 
                            Lista_height_Atual.append(h) 
                            Lista_width_Atual.append(w)                
                            x_anterior = x
                            y_anterior = y
                            cv2.drawContours(img2, [contorno], 0, (200,69,30), 3)
                            j +=1                     

                        else:
                            cv2.drawContours(thrash, [contorno], 0, (255,255,255), 11)                           
                            campo = thrash[y:(y+h), x:(x+w)]                                               
                            cv2.imwrite('Salvos/'+ prefixo + str(contadorImagem) + 'Area' + str(a)+'.png',campo) 
                            Lista_Areas.append(Image.open('Salvos/'+ prefixo + str(contadorImagem) + 'Area' + str(a)+'.png'))                      
                            x_anterior = x
                            y_anterior = y
                            cv2.drawContours(img2, [contorno], 0, (200,69,30), 3)
                            a += 1
                    w_anterior = w     
                    h_anterior = h
                    areaRetangulo_Anterior = areaRetangulo
            contadorHierarq += 1

        
        Lista_campos_Atual.reverse()
        Lista_width_Atual.reverse()    
        Lista_height_Atual.reverse()
        Lista_campos_Final.append(Lista_campos_Atual)
        Lista_width_Final.append(Lista_width_Atual)
        Lista_height_Final.append(Lista_height_Atual)
        
        if (criarUmTxtPorImagem == True):
            contadorArea += 1
            CriaTXT(Lista_width_Final, Lista_height_Final, Lista_campos_Final, 'resultadoArea'+ str(contadorArea))

    if (criarUmTxtPorImagem == False):
        CriaTXT(Lista_width_Final, Lista_height_Final, Lista_campos_Final, 'resultadoCamposAvulsos')        
        #cv2.imwrite('Salvosteste/pacamposteste.jpg',img2)  
               
def CriaTXT(Lista_width_Final, Lista_height_Final, Lista_campos_Final, prefixoResultado):
    global contadorTXT
    contadorTXT += 1 
    eixoX = int(max(Lista_width_Final[0]))   
    eixoY = int(sum(Lista_height_Final[0]))
    new_img = Image.new("RGB", (eixoX, eixoY), (255,255,255)) 
    new_img.save('Salvos/CamposConcat'+str(contadorTXT)+'.jpg') 
    soma_altura = 0 
    k = 0 
    
    for campo in Lista_campos_Final[0]: 
        new_img.paste(campo, (0, soma_altura)) 
        soma_altura += Lista_height_Final[0][k]
        k += 1 
    new_img.save('Salvos/CamposConcat'+str(contadorTXT)+'.jpg') 
    texto = pytesseract.image_to_string(cv2.imread('Salvos/CamposConcat'+str(contadorTXT)+'.jpg'), lang='por') 
    texto = texto.replace('|','').replace('[','').replace(']','').replace('(','').replace(')','').replace(';','').replace('ª','').replace('\x0C','')
    texto = "".join([s for s in texto.strip().splitlines(True) if s.strip()])    
    #print (texto) 
    f = open(prefixoResultado + '.txt', 'w+') 
    f.write(texto)
    Lista_campos_Final.clear() 
    Lista_width_Final.clear()
    Lista_height_Final.clear() 
    


#paginas = convert_from_path(caminhoPac, 400, poppler_path=poppler_path)

pagina2 = Image.open('pagina10.jpg')
imagens = []
imagens.append(pagina2)
contadorTXT=0


valorMinimoAreaRelativa = 15
Lista_campos_Final = []
Lista_width_Final = []
Lista_height_Final = []
Lista_Areas = []
criarUmTxtPorImagem = False
Leitura("pagina", imagens)


valorMinimoAreaRelativa = 5
contadorArea = 0
criarUmTxtPorImagem = True 
Leitura("area", Lista_Areas)    


    


# for campo in Lista_campos_Final:
#     texto = pytesseract.image_to_string(campo, lang='por', config='--psm 10') 
#     texto = texto.replace('|','').replace('[','').replace(']','').replace('(','').replace(')','').replace(';','').replace('ª','').replace('\x0C','').replace('\n','')
#     texto = "".join([s for s in texto.strip().splitlines(True) if s.strip()])   
#     Lista_texto.append(texto)
# Lista_texto.reverse()

# f = open("resultadoteste.txt", "w+")

# for texto in Lista_texto:     
#     f.write(texto + '\n')



# crsor = conexao.cursor()
# qery = (("""INSERT INTO TWISTER_CASH (ID_REGISTROCASH,
#                                                                            TIPO_REGISTRO,
#                                                                            ID_AREA_RESPOSTA,
#                                                                            ID_USUARIO_RESPOSTA,
#                                                                             DESTINATARIO_RESPOSTA,
#                                                                             ASSUNTO_RESPOSTA,
#                                                                             NIVEL1,
#                                                                             NIVEL2,
#                                                                             NIVEL3,
#                                                                             ID_PRODUTO,
#                                                                             DATA_RESPOSTA,
#                                                                             HORARIO_RESPOSTA,
#                                                                             FCR,
#                                                                             VERSAO,
#                                                                             DESCRICAO_ARVORE,
#                                                                             ACAO,
#                                                                             FLAG_EMAIL,
#                                                                             DOMINIO_CLIENTE,
#                                                                             CORPO_EMAIL_RESPOSTA)
#                                             VALUES ('{registroID}',
#                                                 '{tipoRegistro}',
#                                                 '{id_area}',
#                                                 '{id_user}',
#                                                 '{registroRecipients}',
#                                                 '{registroSubject}', 
#                                                 '{nivel_1}',
#                                                 '{nivel_2}',
#                                                 '{nivel_3}',
#                                                 '{produto}',
#                                                 '{date}',
#                                                 '{hora}',
#                                                 '{FCR}',
#                                                 '{versao}',
#                                                 '{descricaoArvore}',
#                                                 '{acao}',
#                                                 '{tipoRegistro}',
#                                                 '{dominio}',
#                                                 '{registroCorpoEmail}')
                                        
#                                         """).format(registroSender = registroSender[0:255],
#                                                     registroID = registroID, 
#                                                     tipoRegistro = tipoRegistro,
#                                                     id_area = id_area,
#                                                     id_user = tag,
#                                                     registroRecipients = registroRecipients[0:255],
#                                                     registroSubject = registroSubject[0:255],
#                                                     nivel_1 = nivel_1,
#                                                     nivel_2 = nivel_2,
#                                                     nivel_3 = nivel_3,
#                                                     produto = produto,
#                                                     date = date,
#                                                     hora = hora,
#                                                     FCR = FCR,
#                                                     versao = versao, 
#                                                     descricaoArvore = descricaoArvore,
#                                                     acao = acao,
#                                                     dominio = dominio,
#                                                     registroCorpoEmail = registroCorpoEmail[0:1073741824]))
                    
# cursor.execute(query)                    
# conexao.commit()



	
