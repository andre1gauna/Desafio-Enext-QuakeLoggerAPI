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
# ratio = fuzz.ratio (str1.lower(), str2.lower())

# server = 'sqndsc001'
# db1 = 'DBTB420'
# uname = 'gaunaan'
# pswd = '987123'
# conexao = pyodbc.connect(driver='{​​SQL Server}​​',
#     server = server,
#     database = db1,
#     uid= uname,
#     pwd = pswd)


poppler_path = r'C:\Users\andre\Downloads\Release-20.11.0\poppler-20.11.0\bin'
pytesseract.pytesseract.tesseract_cmd= r'C:\Program Files\Tesseract-OCR\tesseract.exe'


ListaDeComparativos = ['Identificação do Representante Legal','Endereço','Média mensal apurada no período dos últimos doze 12 meses','Dados dos Representantes Itaú Empresas na Internet','Pessoas autorizadas a receber os talões de cheques, assinando o respectivo comprovante na entrega:','Dados Representantes IEI','Devedores solidários','Solicitação de alterações de Domicílio Bancário','Endereço instalação','Estabelecimento localizado em shopping?','Assinatura','DECLARAÇÃO - CONTAS MANTIDAS POR ENTIDADES FATCA/CRS','4 - Dados do Contrador Pessoa Física Residente Fiscal em outro país','DECLARAÇÃO DE EMISSÃO DE AÇÕES AO PORTADOR BEARER SHARES AO BANCO ITAÚ']
def switch_TipoArea(indiceComparativo, ListaLinhasTxt):
    separador = ' '
    
    if (indiceComparativo == 0): # Identificação do Representante Legal / Procurador da Empresa autorizado a movimentar a conta
        ListaLinhasTxt.remove(ListaLinhasTxt[0])
        indexLinha = 0
        CpfRepresentanteLegal = ''
        NomeRepresentanteLegal = ''
        CelularRepresentanteLegal = ''
        TelFixoRepresentanteLegal = ''
        EmailRepresentanteLegal = ''
        for linha in ListaLinhasTxt:
            linhaSplit = linha.split()
            if (fuzz.ratio(linhaSplit[0].lower(), 'cpf:') >= 70 and not CpfRepresentanteLegal):
                linhaSplit.remove(linhaSplit[0])
                CpfRepresentanteLegal = separador.join(linhaSplit)
            elif (fuzz.ratio(linhaSplit[0].lower(), 'nome:') >= 70 and not NomeRepresentanteLegal):
                linhaSplit.remove(linhaSplit[0])
                NomeRepresentanteLegal = separador.join(linhaSplit)   
            elif (fuzz.ratio(linhaSplit[0].lower(), 'celular') >= 70 and not CelularRepresentanteLegal):                
                proximaLinhaSplit = ListaLinhasTxt[indexLinha + 1].split()
                proximaLinhaSplit.remove(proximaLinhaSplit[0])
                CelularRepresentanteLegal = separador.join(proximaLinhaSplit)
            elif (fuzz.ratio(linhaSplit[0].lower(), 'telefone') >= 70 and not TelFixoRepresentanteLegal):                
                proximaLinhaSplit = ListaLinhasTxt[indexLinha + 1].split()
                proximaLinhaSplit.remove(proximaLinhaSplit[0])
                TelFixoRepresentanteLegal = separador.join(proximaLinhaSplit)                
            elif (fuzz.ratio(linhaSplit[0].lower(), 'e-mail:') >= 70 and not EmailRepresentanteLegal):
                linhaSplit.remove(linhaSplit[0])           
                EmailRepresentanteLegal = separador.join(linhaSplit)                      
            indexLinha += 1

    elif (indiceComparativo == 1): # Endereço
        ListaLinhasTxt.remove(ListaLinhasTxt[0])
        indexLinha = 0
        EnderecoOutroLEC = ''
        NumeroOutroLEC = ''
        ComplementoOutroLEC = ''
        CEPOutroLEC = ''
        BairroOutroLEC = ''
        
        for linha in ListaLinhasTxt:
            linhaSplit = linha.split()
            if (fuzz.ratio(linhaSplit[0].lower(), 'endereço:') >= 70 and not EnderecoOutroLEC):
                linhaSplit.remove(linhaSplit[0])
                EnderecoOutroLEC = separador.join(linhaSplit)

            elif (fuzz.ratio(linhaSplit[0].lower(), 'número:') >= 70 and not NumeroOutroLEC):
                linhaSplit.remove(linhaSplit[0])
                NumeroOutroLEC = separador.join(linhaSplit)   

            elif (fuzz.ratio(linhaSplit[0].lower(), 'complemento:') >= 70 and not ComplementoOutroLEC):                
                linhaSplit.remove(linhaSplit[0])           
                ComplementoOutroLEC = separador.join(linhaSplit)   

            elif (fuzz.ratio(linhaSplit[0].lower(), 'cep:') >= 70 and not CEPOutroLEC):
                linhaSplit.remove(linhaSplit[0])           
                CEPOutroLEC = separador.join(linhaSplit)   

            elif (fuzz.ratio(linhaSplit[0].lower(), 'bairro:') >= 70 and not BairroOutroLEC):                
                linhaSplit.remove(linhaSplit[0])           
                BairroOutroLEC = separador.join(linhaSplit)    

            elif (fuzz.ratio(linhaSplit[0].lower(), 'telefone:') >= 70):
                SalvarEmRamal = False
                proximaLinhaSplit = ListaLinhasTxt[indexLinha + 1].split()
                proximaLinhaSplit.remove(proximaLinhaSplit[0])
                TelOutroLEC = ''
                RamalOutroLEC =''
                for item in proximaLinhaSplit:  
                    if (fuzz.ratio(item.lower(), 'ramal:') >= 70):
                        SalvarEmRamal = True          
                    if (SalvarEmRamal == True):
                        RamalOutroLEC += ' ' + item
                    else:              
                        TelOutroLEC += item

                RamalOutroLEC = RamalOutroLEC.split()
                RamalOutroLEC.remove(RamalOutroLEC[0])
                RamalOutroLEC = separador.join(RamalOutroLEC)

            indexLinha += 1

    elif (indiceComparativo == 2): # Média mensal apurada no período dos últimos doze 12 meses
        ListaLinhasTxt.remove(ListaLinhasTxt[0])
        ListaMeses = []
        ListaAnos = []
        ListaValores = []
        Media = ''

        ListaComparativoMeses = ['janeiro','fevereiro','março','abril','maio','junho','julho','agosto','setembro','outubro','novembro','dezembro']

        for linha in ListaLinhasTxt:
        
            linhaSplit = linha.split()
            k = 0
            quebraLacoExterno = False
            while (k<2): #Considera a presença da "Média" em uma das 2 primeiras palavras
                if (fuzz.ratio('média', linhaSplit[k].lower()) >= 70):
                    Media = linhaSplit[k + 1]
                    quebraLacoExterno = True
                    break;
                k += 1    
            if (quebraLacoExterno):
                break;

            linhaSplit.remove(linhaSplit[0]) # Remove a palavra "Mês"
            i = 0
            quebraLacoExterno2 = False
            while (i<2): #considera a presença do mês em uma das 2 próximas palavras da lista
                for comparativoMes in ListaComparativoMeses:
                    if (fuzz.ratio(comparativoMes, linhaSplit[i].lower()) >= 70):
                        ListaMeses.append(comparativoMes)
                        linhaSplit = linhaSplit[i+2:] # ignora a proxima palavra, que seria "Ano:", para facilitar a captura de seu valor, por exemplo "2020"
                        quebraLacoExterno2 = True
                        break;
                if (quebraLacoExterno2):
                    break        
                i += 1
            j = 0    
            while (j<2):    
                if '20' in linhaSplit[j]:
                    ListaAnos.append(linhaSplit[j])
                    break;
                j += 1
            linhaSplit = ''.join(linhaSplit[j+1:])
            ListaValores.append(linhaSplit.replace('R','').replace('$','')) 

    elif (indiceComparativo == 3): # Dados dos Representantes Itaú Empresas na Internet
        ListaLinhasTxt.remove(ListaLinhasTxt[0])
        indexLinha = 0
        TiposAlcada = ['Simples','Dupla', 'Simples & Dupla']
        NomeRepresentanteIEI = ''
        CpfRepresentanteIEI = ''
        CelularRepresentanteIEI = ''
        EmailRepresentanteIEI = ''
        TipoAlcadaIEI = ''
        ValorAlcadaSimplesIEI = ''
        ValorAlcadaDuplaIEI = ''
        for linha in ListaLinhasTxt:
            linhaSplit = linha.split()
            if (fuzz.ratio(linhaSplit[0].lower(), 'nome:') >= 70 and not NomeRepresentanteIEI):
                linhaSplit.remove(linhaSplit[0])
                NomeRepresentanteIEI = separador.join(linhaSplit)
            elif (fuzz.ratio(linhaSplit[0].lower(), 'cpf') >= 70 and not CpfRepresentanteIEI):
                linhaSplit.remove(linhaSplit[0])
                CpfRepresentanteIEI = separador.join(linhaSplit)   
            elif (fuzz.ratio(linhaSplit[0].lower(), 'celular') >= 70 and not CelularRepresentanteIEI):                
                proximaLinhaSplit = ListaLinhasTxt[indexLinha + 1].split()
                proximaLinhaSplit.remove(proximaLinhaSplit[0])
                CelularRepresentanteIEI = separador.join(proximaLinhaSplit)                      
            elif (fuzz.ratio(linhaSplit[0].lower(), 'e-mail:') >= 70 and not EmailRepresentanteIEI):
                linhaSplit.remove(linhaSplit[0])           
                EmailRepresentanteLegal = separador.join(linhaSplit)
            elif (fuzz.ratio(' '.join(linhaSplit).lower(), 'Valor alçada dupla caso determinada') >= 70 and not ValorAlcadaDuplaIEI):                
                proximaLinha = ListaLinhasTxt[indexLinha + 1].replace('R','').replace('$','')        
                ValorAlcadaDuplaIEI = proximaLinha.replace('\n','')  
            elif (fuzz.ratio(' '.join(linhaSplit).lower(), 'Valor alçada simples caso determinada') >= 70 and not ValorAlcadaSimplesIEI):                
                proximaLinha = ListaLinhasTxt[indexLinha + 1].replace('R','').replace('$','')        
                ValorAlcadaSimplesIEI = proximaLinha.replace('\n','')      
            else:
                for tipoAlcada in TiposAlcada:
                    if(fuzz.ratio(linhaSplit[0].lower(), tipoAlcada) >= 70 and not TipoAlcadaIEI):
                        linhaSplit.remove(linhaSplit[0])           
                        TipoAlcadaIEI = tipoAlcada
                        break                          
            indexLinha += 1   

    elif (indiceComparativo == 4): # Pessoas autorizadas a receber os talões de cheques, assinando o respectivo comprovante na entrega
        ListaLinhasTxt.remove(ListaLinhasTxt[0])
        indexPalavra = 0
        ListaNomesAutorizados = []
        ListaRGsAutorizados = []
        for linha in ListaLinhasTxt:
            trava = False
            NomePessoa = ''
            RGPessoa = ''
            indexPalavra = 0
            linhaSplit = linha.split()
            linhaSplit.remove(linhaSplit[0])
            for palavra in linhaSplit:
                if (fuzz.ratio(palavra, 'Documento:') >= 70):
                    trava = True 
                if (not trava):     
                    NomePessoa += palavra + ' '   
                if ('RG' in palavra):
                    RGPessoa = linhaSplit[indexPalavra+1:]
                indexPalavra += 1  
            ListaNomesAutorizados.append(NomePessoa)
            ListaRGsAutorizados.append(RGPessoa)   
            
def Leitura(prefixo, imagens):
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
        verificacao = 0
        contadorImagem += 1        
        if (prefixo == 'area'):
            Lista_campos_Final.clear() 
            Lista_width_Final.clear()
            Lista_height_Final.clear()
        else:
            imagem = np.array(imagem)
            imagem = imagem[:,:,0]              
                      
        # paginaAtual = 'Salvos/'+ prefixo + str(contadorImagem) + '.jpg'          
        # imagem.save(paginaAtual, "JPEG") 
        # img = cv2.imread('Salvos/' + prefixo+ str(contadorImagem) + '.jpg', 0) 
        # img2 = cv2.imread('Salvos/' + prefixo+ str(contadorImagem) + '.jpg')
        
        _, thrash = cv2.threshold(imagem, 200, 255, cv2.THRESH_BINARY) 
        contornos, hierarquias = cv2.findContours(thrash, cv2.RETR_LIST, cv2.CHAIN_APPROX_SIMPLE)
        Altura = imagem.shape[0] 
        Largura = imagem.shape[1] 
        AreaTotal = Altura*Largura              
        j = 0
        a = 0
        Lista_campos_Atual = []
        Lista_width_Atual =[] 
        Lista_height_Atual = []              
        ContornosValidos = []
        Lista_ContornosDeAreas = []
        
        while (verificacao < 2):
            for contorno in contornos:             
                contornoCorrigido = cv2.approxPolyDP(contorno, 0.015* cv2.arcLength(contorno,True), True)
            
                if len(contornoCorrigido) == 4:                 
                    x,y,w,h = cv2.boundingRect(contornoCorrigido)
                    # if (paginaAtual=='Salvos/pagina15.jpg'):            
                    #     if (x in range(5,15) and y in range(95,105)):
                    #         popa=10 
                    if y==0: 
                        y=1 
                    if x==0: 
                        x=1                 
                    areaRetangulo = w*h 
                    areaRelativa = AreaTotal/areaRetangulo 
                    
                    if (areaRelativa < 3000 and areaRelativa > 1.5):            
                        if (areaRelativa < valorMinimoAreaRelativa and verificacao == 0): # garante que o contorno é uma área que contem campos, e não um campo avulso.                          
                            cv2.drawContours(thrash, [contorno], 0, (255,255,255), 9)
                            #cv2.imwrite('Salvos/trash.png', thrash)                           
                            Area = thrash[y:(y+h), x:(x+w)].copy()                            
                            if(not np.mean(Area) == 255):                                               
                                #cv2.imwrite('Salvos/'+ prefixo + str(contadorImagem) + 'Area' + str(a)+'.png', Area)
                                Lista_Areas.append(Area) 
                                cv2.drawContours(thrash, [contorno], 0, (255,255,255), -1) 
                                                                             
                                #cv2.drawContours(img2, [contorno], 0, (200,69,30), 3)                        
                                a += 1
                                #cv2.imwrite('Salvos/trashAreaFilled.png', thrash)
                                

                        elif (areaRelativa > valorMinimoAreaRelativa and verificacao > 0):                         
                            cv2.drawContours(thrash, [contorno], 0, (255,255,255), 9)
                            #cv2.imwrite('Salvos/trash.png', thrash)                           
                            campo = thrash[y:(y+h), x:(x+w)].copy()                           
                            if(not np.mean(campo) == 255):
                                #cv2.imwrite('Salvos/'+ prefixo  +str(contadorImagem) +'campo'+str(j)+'.png',campo)
                                 
                                Lista_campos_Atual.append(campo) 
                                Lista_height_Atual.append(h) 
                                Lista_width_Atual.append(w)
                                cv2.drawContours(thrash, [contorno], 0, (255,255,255), -1)                                                                               
                                #cv2.drawContours(img2, [contorno], 0, (200,69,30), 3)
                                j += 1                                
                                #cv2.imwrite('Salvos/trashAreaFilled.png', thrash)
                                                  

            if (verificacao > 0):
                Lista_campos_Atual.reverse()
                Lista_width_Atual.reverse()    
                Lista_height_Atual.reverse()
                Lista_campos_Final += Lista_campos_Atual
                Lista_width_Final += Lista_width_Atual
                Lista_height_Final += Lista_height_Atual

            if (criarUmTxtPorImagem == True and verificacao > 0):    
                CriaTXT(Lista_width_Final, Lista_height_Final, Lista_campos_Final, 'resultadoArea'+ str(contadorArea))
                contadorArea += 1

            verificacao += 1
            contadortemporario = 0
           

    if (criarUmTxtPorImagem == False):
        CriaTXT(Lista_width_Final, Lista_height_Final, Lista_campos_Final, 'resultadoCamposAvulsos')        
        #cv2.imwrite('Salvosteste/pacamposteste.jpg',img2)  
               
def CriaTXT(Lista_width_Final, Lista_height_Final, Lista_campos_Final, prefixoResultado):
    global contadorTXT 
    eixoX = int(max(Lista_width_Final))   
    eixoY = int(sum(Lista_height_Final))
    new_img = Image.new("RGB", (eixoX, eixoY), (255,255,255)) 
    #new_img.save('Salvos/CamposConcat'+str(contadorTXT)+'.jpg')
    contadorTXT += 1  
    soma_altura = 0 
    k = 0 
    
    for campo in Lista_campos_Final:
        campo = Image.fromarray(campo) 
        new_img.paste(campo, (0, soma_altura)) 
        soma_altura += Lista_height_Final[k]
        k += 1
    if (prefixoResultado == 'resultadoArea0'):       
        new_img.save('Salvos/CamposConcat'+str(contadorTXT)+'.jpg')
        texto = pytesseract.image_to_string(cv2.imread('Salvos/CamposConcat'+str(contadorTXT)+'.jpg'), lang='por')
             
    #new_img.save('Salvos/CamposConcat'+str(contadorTXT)+'.jpg') 
    
    texto = pytesseract.image_to_string(new_img, lang='por', config = '--psm 6 --oem 2')  
    texto = texto.replace('|','').replace('[','').replace(']','').replace('(','').replace(')','').replace(';','').replace('ª','').replace('\x0C','')
    texto = "".join([s for s in texto.strip().splitlines(True) if s.strip()])    
  
    f = open('Salvos/' + prefixoResultado + '.txt', 'w+') 
    f.write(texto)
    


# pagina2 = Image.open('pagina10.jpg')
# imagens = []
# imagens.append(pagina2)


imagens = convert_from_path('testeV1.3.pdf', 400, poppler_path=poppler_path, fmt='jpeg')
#sys.exit()
contadorTXT=0
# contadorArea = 0

valorMinimoAreaRelativa = 15
Lista_campos_Final = []
Lista_width_Final = []
Lista_height_Final = []
Lista_Areas = []
# criarUmTxtPorImagem = True
criarUmTxtPorImagem = False
Leitura("pagina", imagens)

 
valorMinimoAreaRelativa = 1.5
contadorArea = 0
criarUmTxtPorImagem = True 
Leitura("area", Lista_Areas)    


# while(contadorArea>0):
#     ListaLinhasTxt = open('Salvos/resultadoArea'+ str(contadorArea) +'.txt').readlines()
#     for linha in ListaLinhasTxt:
#         indiceComparativo = 0
#         for comparativo in ListaDeComparativos:
#             assertividade = fuzz.ratio (comparativo.lower(), linha.lower())
#             if assertividade >= 70 :
#                 switch_TipoArea(indiceComparativo, ListaLinhasTxt)
#                 break;
#             indiceComparativo += 1    
#     contadorArea -= 1

# cursor = conexao.cursor()
# query = (("""INSERT INTO TWISTER_CASH (ID_REGISTROCASH,
#                                                                             TIPO_REGISTRO,
#                                                                             ID_AREA_RESPOSTA,
#                                                                             ID_USUARIO_RESPOSTA,
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

