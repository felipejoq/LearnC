using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;               
using System.Net.Sockets;
using ServidorEcho.Modelos;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
 
namespace Servidor
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
            IPEndPoint miDireccion = new IPEndPoint(IPAddress.Any, 2048);

            try
            {
                miPrimerSocket.Bind(miDireccion);

                List<Usuario> _listaUsuarios = new List<Usuario>();

                Escuchando:
                miPrimerSocket.Listen(1);
                Console.WriteLine("Esperando la conexión del cliente...");
                Socket Escuchar = miPrimerSocket.Accept();
                Console.WriteLine("Conectado con exito");
                int i = 0;
                byte[] textoEnviars;
                string[] credenciales = new string[2];
                int opcionCliente = 0;


                //_listaUsuarios.Add(new Usuario("Felipe"));
                //_listaUsuarios.Add(new Usuario("Antonio"));

                do
                {
                    byte[] ByRec = new byte[255];
                    int a = Escuchar.Receive(ByRec, 0, ByRec.Length, 0);
                    Array.Resize(ref ByRec, a);

                    if(i == 0){
                        credenciales[0] = Encoding.Default.GetString(ByRec);
                    }
                    if(i == 1){
                        credenciales[1] = Reverse(Encoding.Default.GetString(ByRec));

                        Console.WriteLine("usuario: "+credenciales[0]);
                        Console.WriteLine("clave: "+credenciales[1]);


                        if(credenciales[0] == credenciales[1]){

                            textoEnviars = Encoding.Default.GetBytes("true"); //pasamos el texto a array de bytes
                            Escuchar.Send(textoEnviars, 0, textoEnviars.Length, 0); // y lo enviamos

                            Console.WriteLine("Las credenciales coinciden.");

                            Console.WriteLine("Comienza el eco! :D");
                            do{
                                Inicio:
                                byte[] ByRec2 = new byte[255];
                                int ab = Escuchar.Receive(ByRec2, 0, ByRec2.Length, 0);
                                Array.Resize(ref ByRec2, ab);
                                Console.WriteLine("Cliente dice: " + Encoding.Default.GetString(ByRec2));

                                try{
                                    opcionCliente = Int32.Parse(Encoding.Default.GetString(ByRec2));
                                }catch(Exception e){
                                    Console.WriteLine(e);
                                    goto Inicio;
                                }

                                switch (opcionCliente)
                                {
                                    case 1:
                                        byte[] textoEnviar = Encoding.Default.GetBytes("Eligió crear usuario");
                                        Console.WriteLine("Servidor envía: " + Encoding.Default.GetString(textoEnviar));
                                        byte[] ByRec3 = new byte[255];
                                        int ab2 = Escuchar.Receive(ByRec3, 0, ByRec3.Length, 0);
                                        Array.Resize(ref ByRec3, ab2);

                                        Console.WriteLine("Cliente dice: " + Encoding.Default.GetString(ByRec3));

                                        string nombreuser = Encoding.Default.GetString(ByRec3);

                                        _listaUsuarios.Add(new Usuario(nombreuser));

                                        textoEnviar = Encoding.Default.GetBytes(Encoding.Default.GetString(ByRec3) + " (Nombre guardado).");

                                        Escuchar.Send(textoEnviar, 0, textoEnviar.Length, 0); // y lo enviamos

                                        textoEnviar = null;
                                        opcionCliente = 0;
                                        ByRec3 = null;
                                        break;

                                    case 2:
                                        Console.WriteLine("El cliente quiere la lista.");

                                        if(_listaUsuarios.Count == 0){
                                            textoEnviar = Encoding.Default.GetBytes("La lista está vacía."); //pasamos el texto a array de bytes
                                            Escuchar.Send(textoEnviar, 0, textoEnviar.Length, 0);
                                        }else{
                                            string lista = retornaLista(_listaUsuarios);

                                            Console.WriteLine(lista);
                                            //Enviar la lista
                                            textoEnviar = Encoding.Default.GetBytes(lista); //pasamos el texto a array de bytes
                                            Escuchar.Send(textoEnviar, 0, textoEnviar.Length, 0);
                                        }


                                        break;

                                    case 3:
                                        if(_listaUsuarios.Count == 0){
                                            textoEnviars = Encoding.Default.GetBytes("Lista vacía"); //pasamos el texto a array de bytes
                                            Escuchar.Send(textoEnviars, 0, textoEnviars.Length, 0); // y lo enviamos
                                        }else{
                                            textoEnviars = Encoding.Default.GetBytes("Ingrese ID del usuario a eliminar:"); //pasamos el texto a array de bytes
                                            Escuchar.Send(textoEnviars, 0, textoEnviars.Length, 0); // y lo enviamos

                                            //Escuchamos la id que quiere eliminar
                                            byte[] ByRec55 = new byte[255];
                                            int ab55 = Escuchar.Receive(ByRec55, 0, ByRec55.Length, 0);
                                            Array.Resize(ref ByRec55, ab55);

                                            int id = int.Parse(Encoding.Default.GetString(ByRec55));

                                            try
                                            {
                                                _listaUsuarios.RemoveAt(id);
                                            }
                                            catch (Exception)
                                            {
                                                Console.WriteLine("Hubo un error, vuelve al inicio.");
                                                goto Inicio;
                                            }


                                            //Confirmamos que se eliminó.
                                            textoEnviars = Encoding.Default.GetBytes("Usuario eliminado."); //pasamos el texto a array de bytes
                                            Escuchar.Send(textoEnviars, 0, textoEnviars.Length, 0); // y lo enviamos

                                            foreach (Usuario v in _listaUsuarios)
                                            {
                                                Console.WriteLine(v.Nombre);
                                            }

                                            goto Inicio;
                                        }

                                        break;
                                    default:
                                        textoEnviars = Encoding.Default.GetBytes("Adios"); //pasamos el texto a array de bytes
                                        Escuchar.Send(textoEnviars, 0, textoEnviars.Length, 0); // y lo enviamos
                                        Escuchar.Close();
                                        goto Escuchando;
                                        break;
                                }




                            }while(true);
                        }else{
                            textoEnviars = Encoding.Default.GetBytes("Las credenciales No coinciden."); //pasamos el texto a array de bytes
                            Escuchar.Send(textoEnviars, 0, textoEnviars.Length, 0); // y lo enviamos
                            Escuchar.Close();
                            goto Escuchando;
                        }

                    }


                    i = i + 1;


                } while (true);

            }
            catch (Exception error)
            {
                Console.WriteLine("Error: {0}", error.ToString());
            }
            Console.WriteLine("Presione cualquier tecla para terminar");
            Console.ReadLine();

        }

        public static string Reverse(string text)
        {
            if (text == null) return null;

            // this was posted by petebob as well 
            char[] array = text.ToCharArray();
            Array.Reverse(array);
            return new String(array);
        }

        public static string retornaLista(List<Usuario> lista){

            string cadena = "";
            int contar = -1;

            foreach(Usuario u in lista){
                contar++;
                cadena = contar + "). " + u.Nombre + "\n" + cadena;
            }

            return cadena;
        }
    }
}
