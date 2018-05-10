using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;            //   Paso 1
using System.Net.Sockets;    //   Paso 1
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Cliente
{
    class Program
    {
        static void Main(string[] args)
        {
            Conectar();
        }
        public static void Conectar()
        {
            Socket miPrimerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint miDireccion = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2048);

            string texto;
            byte[] textoEnviar;
            string respuesta = "";



            try
            {
                miPrimerSocket.Connect(miDireccion);
                Console.WriteLine("Conectado con exito");
                byte[] ByRec = new byte[255];
                do
                {
                    for (int i = 0; i < 2; i++){
                        if(i == 0){
                            
                            Console.WriteLine("Ingrese su usuario: ");
                            texto = Console.ReadLine(); //leo el texto ingresado 
                            textoEnviar = Encoding.Default.GetBytes(texto); //pasamos el texto a array de bytes
                            miPrimerSocket.Send(textoEnviar, 0, textoEnviar.Length, 0); // y lo enviamos
                            Console.WriteLine("Usuario enviado exitosamente");
                            //int a = miPrimerSocket.Receive(ByRec, 0, ByRec.Length, 0);
                            //Array.Resize(ref ByRec, a);
                            //Console.WriteLine("Servidor responde: " + Encoding.Default.GetString(ByRec)); //mostramos lo recibido

                        } else if (i == 1) {
                            
                            Console.WriteLine("Ingrese su clave: ");
                            texto = Console.ReadLine(); //leo el texto ingresado 
                            textoEnviar = Encoding.Default.GetBytes(texto); //pasamos el texto a array de bytes
                            miPrimerSocket.Send(textoEnviar, 0, textoEnviar.Length, 0); // y lo enviamos
                            Console.WriteLine("Clave enviada exitosamente");
                            int a = miPrimerSocket.Receive(ByRec, 0, ByRec.Length, 0);
                            Array.Resize(ref ByRec, a);
                            respuesta = Encoding.Default.GetString(ByRec);
                            Console.WriteLine("Servidor responde: " + Encoding.Default.GetString(ByRec)); //mostramos lo recibido

                        }

                    }
                     if(respuesta == "true"){
                        do{
                            Inicio:
                            //Muestra el menú de ociones
                            Menu.MenuUsuario._menuUsuario();

                            //Crea el arreglo de bytes a enviar.
                            byte[] ByRec2 = new byte[255];
                            byte[] textoEnvia;
                            //Le pide al usuario ingresar su opción, entero.
                            Console.WriteLine("Ingrese su opción: ");
                            texto = Console.ReadLine(); //leo el texto ingresado 
                            textoEnvia = Encoding.Default.GetBytes(texto); //pasamos el texto a array de bytes
                            miPrimerSocket.Send(textoEnvia, 0, textoEnvia.Length, 0); // le envío la opción al cliente.
                            Console.WriteLine("Opcion enviada"); //Confirmamos al cliente que la opción fue enviada.

                            switch (int.Parse(texto))
                            {
                                case 1:
                                    Console.WriteLine("Ingrese el nombre del usuario: "); //Pide ingresar el nombre del usuario
                                    texto = Console.ReadLine(); //leo el texto ingresado 
                                    textoEnviar = Encoding.Default.GetBytes(texto); //pasamos el texto a array de bytes
                                    miPrimerSocket.Send(textoEnviar, 0, textoEnviar.Length, 0); // le envío la opción al cliente.
                                    Console.WriteLine("Nombre guardado"); //Confirmamos al cliente que la opción fue enviada.

                                    int ab = miPrimerSocket.Receive(ByRec2, 0, ByRec2.Length, 0); // se recibe el texto desde el servidor.
                                    Array.Resize(ref ByRec2, ab);
                                    Console.WriteLine("Servidor responde: " + Encoding.Default.GetString(ByRec2)); //mostramos lo recibido
                                    Console.WriteLine("");
                                    break;

                                case 2:
                                    Console.WriteLine("Elegí la lista de usuarios");

                                    byte[] malditaLista = new byte[255];
                                    int ab64 = miPrimerSocket.Receive(malditaLista, 0, malditaLista.Length, 0);
                                    Array.Resize(ref malditaLista, ab64);
                                    Console.WriteLine("Servidor envía: \n" + Encoding.Default.GetString(malditaLista));


                                    break;
                                case 3:
                                    byte[] B2 = new byte[255];
                                    int ab66 = miPrimerSocket.Receive(B2, 0, B2.Length, 0);
                                    Array.Resize(ref B2, ab66);
                                    Console.WriteLine("Servidor envía: " + Encoding.Default.GetString(B2));

                                    if (Encoding.Default.GetString(B2) == "Lista vacía")
                                    {
                                        goto Inicio;
                                    }else{
                                        texto = Console.ReadLine(); //leo el texto ingresado 
                                        textoEnviar = Encoding.Default.GetBytes(texto); //pasamos el texto a array de bytes
                                        miPrimerSocket.Send(textoEnviar, 0, textoEnviar.Length, 0);

                                        int ab665 = miPrimerSocket.Receive(B2, 0, B2.Length, 0);
                                        Array.Resize(ref B2, ab665);
                                        Console.WriteLine("Servidor envía: " + Encoding.Default.GetString(B2));
                                    }
                                    break;
                                default:
                                    byte[] B23 = new byte[255];
                                    int ab662 = miPrimerSocket.Receive(B23, 0, B23.Length, 0);
                                    Array.Resize(ref B23, ab662);
                                    Console.WriteLine("Servidor envía: " + Encoding.Default.GetString(B23));
                                    goto Adios;
                                    break;
                            }


                        }while(true);
                    }else{
                        Console.WriteLine("Lo lamento, usted no tiene permisos para hacer eco");
                    }

                } while (false);

                miPrimerSocket.Close();
            }
            catch (Exception error)
            {
                Console.WriteLine("Error: {0}", error.ToString());
            }
            Adios:
            Console.WriteLine("Presione cualquier tecla para terminar");
            Console.ReadLine();
        }
    }
}