using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace proyecto_graficacion
{
    public partial class frm_principal : Form
    {
        Graphics g;
        Pen p;
        SolidBrush b;
        public frm_principal()
        {
            InitializeComponent();
        }

        private void btn_salvar_Click(object sender, EventArgs e)
        {
            try
            {
                //Objeto de tipo SaveFileDialog
                SaveFileDialog sf = new SaveFileDialog
                {
                    Filter = "Bitmap Image (.bmp)|*.bmp|Gif Image (.gif)|*.gif|JPEG Image (.jpeg)|*.jpeg|Png Image (.png)|*.png|Tiff Image (.tiff)|*.tiff|Wmf Image (.wmf)|*.wmf"
                };

                //Si la respuesta del dialogo es afirmativa
                if (sf.ShowDialog() == DialogResult.OK)
                {
                    //Ruta a guardar, ya incluye el nombre
                    var ruta = sf.FileName;

                    //Ancho y alto del panel
                    int ancho = panel_fondo.Size.Width;
                    int alto = panel_fondo.Size.Height;

                    Bitmap bm = new Bitmap(ancho, alto);
                    //Convertimos bitmap a imagen png y guardamos
                    panel_fondo.DrawToBitmap(bm, new Rectangle(0, 0, ancho, alto));
                    //Guardamos
                    bm.Save(ruta, ImageFormat.Bmp);
                }
            }
            catch (Exception exception)
            {
                //Msj de error
                MessageBox.Show("Error al guardar la imagen: " + exception, "ERROR!");
            }
        }

        private void panel_fondo_Paint(object sender, PaintEventArgs e)
        {
            panel_fondo.BackgroundImage = null;

            //Dibujamos fondo
            DibujarFondo(e);
            DibujarIsla(e);
            DibujarBarco(e);
        }

        private void pb_servbot_Paint(object sender, PaintEventArgs e)
        {
            pb_servbot.Image = null;

            //Dibujamos servbot
            DibujarCabeza(e);
            DibujarCara(e);
            DibujarTorso(e);
            DibujarPiernaIzquierda(e);
            DibujarPiernaDerecha(e);
            DibujarBrazoIzquierdo(e);
            DibujarBrazoIzquierdo2(e);
            DibujarBrazoDerecho(e);
            DibujarBrazoDerecho2(e);

            ColorearServbot();
        }

        /// <summary>
        /// Método de apoyo para FloodFill. Comprueba si el color del pixel es igual al color objetivo a cambiar.
        /// </summary>
        /// <param name="a">Color del pixel</param>
        /// <param name="b">Color objetivo a cambiar</param>
        /// <returns></returns>
        private static bool ColorMatch(Color a, Color b)
        {
            return (a.ToArgb() & 0xffffff) == (b.ToArgb() & 0xffffff);
        }

        /// <summary>
        /// FloodFill; Método para colorear por medio de un algoritmo FloodFill.
        /// </summary>
        /// <param name="bmp">Bitmap a colorear</param>
        /// <param name="pt">Punto donde comenzar</param>
        /// <param name="targetColor">Color objetivo a cambiar</param>
        /// <param name="replacementColor">Color a colocar</param>
        static void FloodFill(Bitmap bmp, Point pt, Color targetColor, Color replacementColor)
        {
            //Cola de puntos
            Queue<Point> q = new Queue<Point>();
            //Añadimos el punto proporcionado, punto de partida.
            q.Enqueue(pt);
            //Mientra haya puntos en la cola
            while (q.Count > 0)
            {
                //Para fines de lenguaje punto = pixel, puntos = pixeles
                //Obtenemos el primer punto de la cola
                Point n = q.Dequeue();
                //Si el color del pixel es diferente al color objetivo continuamos.
                if (!ColorMatch(bmp.GetPixel(n.X, n.Y), targetColor))
                    continue;
                //Copiamos el punto, y creamos otro punto del pixel vecino a la derecha
                Point w = n, e = new Point(n.X + 1, n.Y);
                /*Mientras la coordenada en x del punto sea positiva o cero [dentro del cuadrante pantalla] 
                y el color del pixel sea diferente al color objetivo*/
                while ((w.X >= 0) && ColorMatch(bmp.GetPixel(w.X, w.Y), targetColor))
                {
                    //Remplazamos el color del pixel por el color objetivo
                    bmp.SetPixel(w.X, w.Y, replacementColor);
                    /*Si la coordenada en Y es mayor a cero y el color del pixel de arriba sea diferente al color objetivo
                    agremos dicho punto [de arriba] a la cola*/
                    if ((w.Y > 0) && ColorMatch(bmp.GetPixel(w.X, w.Y - 1), targetColor))
                        q.Enqueue(new Point(w.X, w.Y - 1));
                    /*Si la coordenada en Y es menor al tamaño del bitmap y el color del pixel de abajo sea 
                    diferente al color objetivo agremos dicho punto [de abajo] a la cola*/
                    if ((w.Y < bmp.Height - 1) && ColorMatch(bmp.GetPixel(w.X, w.Y + 1), targetColor))
                        q.Enqueue(new Point(w.X, w.Y + 1));
                    //No vemos al pixel de la izquierda
                    w.X--;
                }
                /*Mientras la coordenada en x del punto vecino a la derecha sea menor igual al ancho de la imagen - 1
                y el color de dicho pixel sea diferente al color objetivo*/
                while ((e.X <= bmp.Width - 1) && ColorMatch(bmp.GetPixel(e.X, e.Y), targetColor))
                {
                    //Remplazamos el color del pixel por el color objetivo
                    bmp.SetPixel(e.X, e.Y, replacementColor);
                    /*Si la coordena en y del pixel vecino a la derecha sea mayor a cero y el color del pixel 
                     de arriba sea diferente al color objetivo, agregamos dicho punto [de arriba] a la cola*/
                    if ((e.Y > 0) && ColorMatch(bmp.GetPixel(e.X, e.Y - 1), targetColor))
                        q.Enqueue(new Point(e.X, e.Y - 1));
                    /*Si la coordenada en y es menor al alto de la bitmap - 1 y el color del pixel de abajo sea
                     diferente al color objetivo, agregamos dicho punto [el de abajo] a la cola*/
                    if ((e.Y < bmp.Height - 1) && ColorMatch(bmp.GetPixel(e.X, e.Y + 1), targetColor))
                        q.Enqueue(new Point(e.X, e.Y + 1));
                    //No pasamos al pixel de la derecha
                    e.X++;
                }
                /*Hemos terminado la fila de pixeles. Según sea el siguiente punto en la cola, podemos irnos
                 hacia arriba o hacia abajo en el bitmap*/
            }
        }

        private void DibujarCabeza(PaintEventArgs e)
        {
            g = e.Graphics;
            //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            p = new Pen(Color.Black, 2);
            Point inicio, punto_control_uno, punto_control_dos, final;

            //Cabeza - Linea superior
            inicio = new Point(29, 90);
            punto_control_uno = new Point(64, 34);
            punto_control_dos = new Point(165, 19);
            final = new Point(207, 60);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            //Cabeza - Linea Izquierda
            inicio = new Point(29, 88);
            punto_control_uno = new Point(38, 158);
            punto_control_dos = new Point(26, 186);
            final = new Point(68, 203);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            //Cabeza - Linea inferior
            inicio = new Point(94, 208);
            punto_control_uno = new Point(209, 193);
            punto_control_dos = new Point(258, 211);
            final = new Point(207, 60);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            //Casco(?)
            inicio = new Point(71, 51);
            punto_control_uno = new Point(73, 15);
            punto_control_dos = new Point(155, 15);
            final = new Point(155, 37);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            //Sombreado izquierdo del casco
            p = new Pen(Color.FromArgb(252, 255, 166), 1);
            inicio = new Point(130, 24);
            final = new Point(132, 35);
            g.DrawLine(p, inicio, final);

            //Sombreado derecho del casco
            inicio = new Point(138, 26);
            final = new Point(141, 35);
            g.DrawLine(p, inicio, final);
        }

        private void DibujarCara(PaintEventArgs e)
        {
            g = e.Graphics;
            //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            p = new Pen(Color.Black, 2);
            SolidBrush b = new SolidBrush(Color.Black);
            SolidBrush w = new SolidBrush(Color.White);
            Point inicio, punto_control_uno, punto_control_dos, final;

            //Boca - Curva superior
            inicio = new Point(141, 142);
            punto_control_uno = new Point(150, 144);
            punto_control_dos = new Point(167, 142);
            final = new Point(173, 139);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            //Boca - Curva inferior
            inicio = new Point(142, 142);
            punto_control_uno = new Point(142, 200);
            punto_control_dos = new Point(188, 192);
            final = new Point(172, 139);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            // Ojo derecho contorno
            g.DrawEllipse(p, 68, 70, 68, 68);
            // Ojo derecho interior
            g.FillEllipse(w, 68, 70, 68, 68);
            // Pupila derecha
            g.FillEllipse(b, 81, 82, 41, 41);
            // Ojo Izquierdo contorno
            g.DrawEllipse(p, 163, 67, 50, 62);
            // Ojo Izquierdo interior
            g.FillEllipse(w, 163, 67, 50, 62);
            // Pupila izquierda
            g.FillEllipse(b, 172, 76, 35, 42);

            //Sombreado inferior
            p = new Pen(Color.FromArgb(254, 173, 19), 1);
            inicio = new Point(44, 183);
            punto_control_uno = new Point(72, 199);
            punto_control_dos = new Point(163, 204);
            final = new Point(226, 161);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);
        }
        private void DibujarTorso(PaintEventArgs e)
        {
            g = e.Graphics;
            //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            p = new Pen(Color.Black, 2);

            Point inicio, punto_control_uno, punto_control_dos, final;

            // Torso - Linea inferior izq
            inicio = new Point(86, 316);
            final = new Point(108, 322);
            g.DrawLine(p, inicio, final);

            // Torso - Linea inferior
            inicio = new Point(107, 322);
            punto_control_uno = new Point(131, 327);
            punto_control_dos = new Point(197, 308);
            final = new Point(215, 298);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            // Torso - Linea inferior
            inicio = new Point(180, 199);
            punto_control_uno = new Point(196, 238);
            punto_control_dos = new Point(210, 271);
            final = new Point(215, 299);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            // Botón izquierdo
            g.DrawEllipse(p, 120, 273, 34, 40);

            // Botón Derecho
            g.DrawEllipse(p, 178, 261, 31, 40);

            // Hombro Izquierdo
            g.DrawEllipse(p, 50, 202, 53, 53);

            // Hombro Derecho
            inicio = new Point(195, 237);
            punto_control_uno = new Point(219, 232);
            punto_control_dos = new Point(233, 211);
            final = new Point(205, 190);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            // Linea de torso
            p = new Pen(Color.Black, 1);
            inicio = new Point(109, 247);
            final = new Point(111, 297);
            g.DrawLine(p, inicio, final);

            //Sombreado superior
            p = new Pen(Color.FromArgb(55, 73, 135), 1);
            inicio = new Point(110, 259);
            punto_control_uno = new Point(142, 275);
            punto_control_dos = new Point(183, 256);
            final = new Point(188, 222);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            //Sombreado izquierdo
            final = new Point(112, 322);
            g.DrawLine(p, inicio, final);

            //Sombreado boton izquierdo
            p = new Pen(Color.FromArgb(255, 175, 18), 1);
            inicio = new Point(122, 298);
            punto_control_uno = new Point(129, 307);
            punto_control_dos = new Point(145, 307);
            final = new Point(154, 294);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            //Sombreado boton derecho
            inicio = new Point(181, 288);
            punto_control_uno = new Point(191, 290);
            punto_control_dos = new Point(205, 287);
            final = new Point(208, 276);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            //Sombreado hombro derecho
            p = new Pen(Color.FromArgb(132, 199, 242), 1);
            inicio = new Point(192, 196);
            punto_control_uno = new Point(193, 209);
            punto_control_dos = new Point(205, 219);
            final = new Point(219, 210);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            //Sombreado hombro izquierdo
            inicio = new Point(75, 203);
            punto_control_uno = new Point(50, 224);
            punto_control_dos = new Point(60, 240);
            final = new Point(64, 251);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);
        }
        private void DibujarPiernaDerecha(PaintEventArgs e)
        {
            g = e.Graphics;
            //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            p = new Pen(Color.Black, 2);

            Point inicio, punto_control_uno, punto_control_dos, final;

            //pre rodilla
            inicio = new Point(191, 309);
            final = new Point(194, 341);
            g.DrawLine(p, inicio, final);

            inicio = new Point(161, 329);
            final = new Point(161, 347);
            g.DrawLine(p, inicio, final);

            inicio = new Point(161, 345);
            punto_control_uno = new Point(163, 353);
            punto_control_dos = new Point(196, 351);
            final = new Point(194, 340);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            //gemelo
            inicio = new Point(161, 335);
            punto_control_uno = new Point(149, 342);
            punto_control_dos = new Point(149, 362);
            final = new Point(146, 368);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            inicio = new Point(193, 332);
            punto_control_uno = new Point(213, 338);
            punto_control_dos = new Point(207, 367);
            final = new Point(213, 385);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            inicio = new Point(149, 395);
            punto_control_uno = new Point(162, 409);
            punto_control_dos = new Point(206, 406);
            final = new Point(212, 385);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            //pie
            inicio = new Point(161, 329);
            final = new Point(161, 347);
            g.DrawLine(p, inicio, final);

            inicio = new Point(161, 345);
            punto_control_uno = new Point(163, 353);
            punto_control_dos = new Point(196, 351);
            final = new Point(194, 340);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            inicio = new Point(213, 385);
            final = new Point(227, 396);
            g.DrawLine(p, inicio, final);

            inicio = new Point(158, 420);
            punto_control_uno = new Point(190, 435);
            punto_control_dos = new Point(223, 427);
            final = new Point(227, 395);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            inicio = new Point(227, 395);
            punto_control_uno = new Point(231, 397);
            punto_control_dos = new Point(229, 421);
            final = new Point(230, 433);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            inicio = new Point(157, 446);
            punto_control_uno = new Point(186, 466);
            punto_control_dos = new Point(220, 454);
            final = new Point(230, 432);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            //sombreado

            p = new Pen(Color.FromArgb(55, 73, 135), 1);

            inicio = new Point(186, 456);
            punto_control_uno = new Point(188, 449);
            punto_control_dos = new Point(202, 429);
            final = new Point(197, 428);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            inicio = new Point(171, 405);
            punto_control_uno = new Point(187, 373);
            punto_control_dos = new Point(186, 338);
            final = new Point(208, 351);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            p = new Pen(Color.FromArgb(130, 203, 238), 1);

            inicio = new Point(171, 404);
            final = new Point(196, 426);
            g.DrawLine(p, inicio, final);
        }

        private void DibujarPiernaIzquierda(PaintEventArgs e)
        {
            g = e.Graphics;
            //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            p = new Pen(Color.Black, 2);

            Point inicio, punto_control_uno, punto_control_dos, final;

            //Panzita
            inicio = new Point(90, 316);
            punto_control_uno = new Point(135, 354);
            punto_control_dos = new Point(189, 318);
            final = new Point(190, 309);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            //pre rodilla
            inicio = new Point(103, 326);
            final = new Point(101, 352);
            g.DrawLine(p, inicio, final);

            inicio = new Point(136, 333);
            final = new Point(134, 353);
            g.DrawLine(p, inicio, final);

            inicio = new Point(101, 351);
            punto_control_uno = new Point(107, 362);
            punto_control_dos = new Point(131, 364);
            final = new Point(134, 352);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            //gemelo
            inicio = new Point(101, 345);
            punto_control_uno = new Point(99, 347);
            punto_control_dos = new Point(94, 347);
            final = new Point(92, 354);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            inicio = new Point(135, 344);
            punto_control_uno = new Point(153, 354);
            punto_control_dos = new Point(147, 403);
            final = new Point(148, 404);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            inicio = new Point(91, 353);
            punto_control_uno = new Point(97, 367);
            punto_control_dos = new Point(93, 385);
            final = new Point(81, 391);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            inicio = new Point(81, 390);
            final = new Point(81, 401);
            g.DrawLine(p, inicio, final);

            inicio = new Point(81, 400);
            punto_control_uno = new Point(95, 420);
            punto_control_dos = new Point(120, 418);
            final = new Point(128, 417);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            inicio = new Point(127, 417);
            punto_control_uno = new Point(136, 416);
            punto_control_dos = new Point(144, 409);
            final = new Point(148, 404);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            //pie
            inicio = new Point(148, 404);
            final = new Point(158, 419);
            g.DrawLine(p, inicio, final);

            inicio = new Point(81, 400);
            final = new Point(71, 417);
            g.DrawLine(p, inicio, final);

            inicio = new Point(71, 417);
            punto_control_uno = new Point(82, 447);
            punto_control_dos = new Point(144, 455);
            final = new Point(156, 418);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            inicio = new Point(71, 417);
            final = new Point(71, 448);
            g.DrawLine(p, inicio, final);

            inicio = new Point(158, 419);
            final = new Point(157, 450);
            g.DrawLine(p, inicio, final);

            inicio = new Point(70, 447);
            punto_control_uno = new Point(97, 480);
            punto_control_dos = new Point(135, 477);
            final = new Point(157, 449);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            //sombra

            p = new Pen(Color.FromArgb(130, 203, 238), 1);

            inicio = new Point(89, 436);
            final = new Point(97, 414);
            g.DrawLine(p, inicio, final);

            p = new Pen(Color.FromArgb(55, 73, 135), 1);

            inicio = new Point(88, 464);
            final = new Point(88, 438);
            g.DrawLine(p, inicio, final);

            inicio = new Point(97, 415);
            punto_control_uno = new Point(101, 407);
            punto_control_dos = new Point(102, 383);
            final = new Point(104, 384);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            inicio = new Point(103, 385);
            punto_control_uno = new Point(116, 362);
            punto_control_dos = new Point(131, 348);
            final = new Point(149, 397);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);
        }
        private void DibujarBrazoIzquierdo(PaintEventArgs e)
        {
            g = e.Graphics;
            //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            p = new Pen(Color.Black, 2);

            Point inicio, punto_control_uno, punto_control_dos, final;

            //linea izquierda de brazo izquierdo
            inicio = new Point(87, 254);
            final = new Point(85, 271);
            g.DrawLine(p, inicio, final);

            //linea derecha de brazo izquierdo
            inicio = new Point(59, 250);
            final = new Point(55, 266);
            g.DrawLine(p, inicio, final);

            //curva superior del brazo izquierdo 
            inicio = new Point(55, 264);
            punto_control_uno = new Point(50, 279);
            punto_control_dos = new Point(88, 286);
            final = new Point(85, 270);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            //curva superior izquierda del brazo izquierdo
            inicio = new Point(55, 264);
            punto_control_uno = new Point(52, 266);
            punto_control_dos = new Point(44, 272);
            final = new Point(43, 276);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            //curva superior izquierda del brazo izquierdo
            inicio = new Point(55, 264);
            punto_control_uno = new Point(52, 266);
            punto_control_dos = new Point(44, 272);
            final = new Point(43, 276);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            //curva superior derecha del brazo izquierdo
            inicio = new Point(85, 270);
            punto_control_uno = new Point(91, 273);
            punto_control_dos = new Point(92, 285);
            final = new Point(92, 287);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            //linea derecha de brazo izquierdo 
            inicio = new Point(92, 286);
            final = new Point(82, 332);
            g.DrawLine(p, inicio, final);

            //linea izquierda de brazo izquierdo
            inicio = new Point(42, 275);
            final = new Point(33, 323);
            g.DrawLine(p, inicio, final);

            //curva  del brazo izquierdo
            inicio = new Point(33, 323);
            punto_control_uno = new Point(32, 332);
            punto_control_dos = new Point(84, 342);
            final = new Point(82, 330);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            //linea brazo izquierdo
            inicio = new Point(71, 336);
            final = new Point(60, 339);
            g.DrawLine(p, inicio, final);

            //curva  del brazo izquierdo
            inicio = new Point(43, 330);
            punto_control_uno = new Point(8, 357);
            punto_control_dos = new Point(65, 379);
            final = new Point(60, 338);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            //curva  del brazo izquierdo
            inicio = new Point(77, 336);
            punto_control_uno = new Point(92, 340);
            punto_control_dos = new Point(104, 381);
            final = new Point(81, 391);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            //curva  del brazo izquierdo
            inicio = new Point(81, 391);
            punto_control_uno = new Point(92, 390);
            punto_control_dos = new Point(60, 397);
            final = new Point(52, 395);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            //curva  del brazo izquierdo
            inicio = new Point(52, 395);
            punto_control_uno = new Point(45, 392);
            punto_control_dos = new Point(53, 397);
            final = new Point(45, 390);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            //linea brazo izquierdo
            inicio = new Point(45, 394);
            final = new Point(46, 372);
            g.DrawLine(p, inicio, final);

            //curva  del brazo izquierdo
            inicio = new Point(46, 372);
            punto_control_uno = new Point(55, 373);
            punto_control_dos = new Point(70, 371);
            final = new Point(53, 359);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            //curva cuerca de la mano izquierdo 1
            inicio = new Point(43, 339);
            punto_control_uno = new Point(50, 338);
            punto_control_dos = new Point(54, 353);
            final = new Point(43, 356);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            //curva cuerca de la mano izquierdo 2
            inicio = new Point(43, 339);
            punto_control_uno = new Point(35, 338);
            punto_control_dos = new Point(32, 353);
            final = new Point(43, 356);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            //linea brazo izquierdo
            inicio = new Point(33, 383);
            final = new Point(46, 385);
            g.DrawLine(p, inicio, final);

            //linea brazo izquierdo
            inicio = new Point(33, 385);
            final = new Point(37, 371);
            g.DrawLine(p, inicio, final);

            //curva cuerca de la mano izquierdo 3
            inicio = new Point(37, 371);
            punto_control_uno = new Point(32, 369);
            punto_control_dos = new Point(27, 358);
            final = new Point(35, 356);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            //curva cuerca de la mano izquierdo 4
            inicio = new Point(37, 371);
            punto_control_uno = new Point(32, 369);
            punto_control_dos = new Point(27, 358);
            final = new Point(35, 356);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            //curva cuerca de la mano izquierdo 5
            inicio = new Point(37, 371);
            punto_control_uno = new Point(32, 369);
            punto_control_dos = new Point(27, 358);
            final = new Point(35, 356);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            //curva cuerca de la mano izquierdo 6
            inicio = new Point(33, 385);
            punto_control_uno = new Point(1, 373);
            punto_control_dos = new Point(12, 328);
            final = new Point(39, 332);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);
        }
        private void DibujarBrazoIzquierdo2(PaintEventArgs e)
        {
            g = e.Graphics;
            //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            p = new Pen(Color.Black, 1);

            Point inicio, punto_control_uno, punto_control_dos, final;

            //curva  del brazo izquierdo
            inicio = new Point(65, 347);
            punto_control_uno = new Point(69, 347);
            punto_control_dos = new Point(73, 351);
            final = new Point(73, 352);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            //sombra
            p = new Pen(Color.FromArgb(192, 201, 158), 1);
            inicio = new Point(66, 255);
            final = new Point(62, 275);
            g.DrawLine(p, inicio, final);

            p = new Pen(Color.FromArgb(55, 73, 135), 1);
            inicio = new Point(55, 270);
            punto_control_uno = new Point(51, 273);
            punto_control_dos = new Point(56, 289);
            final = new Point(55, 289);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            inicio = new Point(55, 288);
            final = new Point(50, 332);
            g.DrawLine(p, inicio, final);

            p = new Pen(Color.FromArgb(254, 231, 13), 1);

            inicio = new Point(73, 352);
            final = new Point(90, 347);
            g.DrawLine(p, inicio, final);

            inicio = new Point(73, 352);
            punto_control_uno = new Point(77, 359);
            punto_control_dos = new Point(83, 370);
            final = new Point(67, 394);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);
        }
        private void DibujarBrazoDerecho(PaintEventArgs e)
        {
            g = e.Graphics;
            //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            p = new Pen(Color.Black, 2);
            Point inicio, punto_control_uno, punto_control_dos, final;

            //linea de la brazo derecha
            inicio = new Point(220, 208);
            final = new Point(230, 196);
            g.DrawLine(p, inicio, final);

            //curva de la brazo derecha
            inicio = new Point(230, 196);
            punto_control_uno = new Point(235, 190);
            punto_control_dos = new Point(226, 177);
            final = new Point(220, 178);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            //curva de la brazo derecha
            inicio = new Point(226, 202);
            punto_control_uno = new Point(232, 203);
            punto_control_dos = new Point(238, 201);
            final = new Point(239, 200);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            //linea de la brazo derecha
            inicio = new Point(239, 200);
            final = new Point(263, 166);
            g.DrawLine(p, inicio, final);

            //curva de la brazo derecha
            inicio = new Point(263, 166);
            punto_control_uno = new Point(264, 154);
            punto_control_dos = new Point(250, 132);
            final = new Point(224, 136);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            //curva del brazo derecha
            inicio = new Point(260, 117);
            punto_control_uno = new Point(273, 121);
            punto_control_dos = new Point(270, 157);
            final = new Point(244, 139);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            //curva de la mano derecha
            inicio = new Point(260, 117);
            punto_control_uno = new Point(239, 109);
            punto_control_dos = new Point(230, 132);
            final = new Point(244, 139);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            //curva de la mano derecha
            inicio = new Point(267, 126);
            punto_control_uno = new Point(276, 129);
            punto_control_dos = new Point(282, 112);
            final = new Point(269, 109);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            //linea de la mano derecha
            inicio = new Point(269, 109);
            final = new Point(268, 118);
            g.DrawLine(p, inicio, final);

            //curva de la mano derecha
            inicio = new Point(268, 118);
            punto_control_uno = new Point(271, 119);
            punto_control_dos = new Point(270, 125);
            final = new Point(270, 126);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            //linea de la mano derecha
            inicio = new Point(269, 109);
            final = new Point(285, 95);
            g.DrawLine(p, inicio, final);

            //curva de la mano derecha
            inicio = new Point(285, 95);
            punto_control_uno = new Point(316, 115);
            punto_control_dos = new Point(312, 167);
            final = new Point(264, 160);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            //curva de la mano derecha
            inicio = new Point(222, 115);
            punto_control_uno = new Point(226, 81);
            punto_control_dos = new Point(257, 80);
            final = new Point(267, 87);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            //linea de la mano derecha
            inicio = new Point(267, 87);
            final = new Point(261, 105);
            g.DrawLine(p, inicio, final);

            //linea de la mano derecha
            inicio = new Point(267, 87);
            final = new Point(268, 106);
            g.DrawLine(p, inicio, final);

            //curva de la mano derecha
            inicio = new Point(268, 106);
            punto_control_uno = new Point(265, 109);
            punto_control_dos = new Point(262, 116);
            final = new Point(262, 119);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            //curva de la mano derecha
            inicio = new Point(261, 105);
            punto_control_uno = new Point(245, 104);
            punto_control_dos = new Point(247, 113);
            final = new Point(247, 116);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            //curva de la mano derecha
            inicio = new Point(257, 120);
            punto_control_uno = new Point(244, 114);
            punto_control_dos = new Point(238, 130);
            final = new Point(248, 133);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            //curva de la mano derecha
            inicio = new Point(257, 120);
            punto_control_uno = new Point(272, 127);
            punto_control_dos = new Point(255, 139);
            final = new Point(248, 133);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);
        }
        private void DibujarBrazoDerecho2(PaintEventArgs e)
        {
            g = e.Graphics;
            //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            p = new Pen(Color.Black, 1);
            Point inicio, punto_control_uno, punto_control_dos, final;

            inicio = new Point(222, 120);
            punto_control_uno = new Point(225, 126);
            punto_control_dos = new Point(229, 128);
            final = new Point(230, 130);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            //linea de la mano derecha
            inicio = new Point(261, 104);
            final = new Point(265, 111);
            g.DrawLine(p, inicio, final);

            //curva de la mano derecha
            inicio = new Point(299, 124);
            punto_control_uno = new Point(299, 132);
            punto_control_dos = new Point(296, 140);
            final = new Point(294, 141);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            p = new Pen(Color.FromArgb(255, 231, 21), 1);

            //linea de la mano derecha
            inicio = new Point(296, 138);
            final = new Point(300, 148);
            g.DrawLine(p, inicio, final);

            //curva de la mano derecha
            inicio = new Point(260, 152);
            punto_control_uno = new Point(270, 155);
            punto_control_dos = new Point(298, 150);
            final = new Point(296, 138);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            //curva de la mano derecha
            p = new Pen(Color.FromArgb(55, 74, 132), 1);
            inicio = new Point(228, 183);
            punto_control_uno = new Point(241, 186);
            punto_control_dos = new Point(258, 163);
            final = new Point(260, 152);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            //curva de la mano derecha
            p = new Pen(Color.FromArgb(193, 213, 162), 1);
            inicio = new Point(216, 183);
            punto_control_uno = new Point(222, 182);
            punto_control_dos = new Point(225, 199);
            final = new Point(219, 204);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

        }

        private void DibujarFondo(PaintEventArgs e)
        {
            g = e.Graphics;
            Point inicio, punto_control_uno, punto_control_dos, final;

            //Cielo
            b = new SolidBrush(Color.FromArgb(255, 150, 215, 233));
            g.FillRectangle(b, 0, 0, 860, 200);

            //Mar
            b = new SolidBrush(Color.FromArgb(255, 18, 161, 201));
            g.FillRectangle(b, 0, 150, 860, 190);

            //Arena
            b = new SolidBrush(Color.FromArgb(255, 230, 217, 162));
            g.FillRectangle(b, 0, 270, 860, 270);

            //Arena mojada

            p = new Pen(Color.FromArgb(255, 211, 187, 117), 15);
            inicio = new Point(0, 273);
            punto_control_uno = new Point(112, 323);
            punto_control_dos = new Point(224, 258);
            final = new Point(300, 297);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            inicio = new Point(297, 297);
            punto_control_uno = new Point(390, 325);
            punto_control_dos = new Point(465, 280);
            final = new Point(528, 273);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            inicio = new Point(528, 273);
            punto_control_uno = new Point(680, 255);
            punto_control_dos = new Point(758, 295);
            final = new Point(860, 268);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            //Arena humeda

            p = new Pen(Color.FromArgb(255, 231, 206, 142), 15);
            inicio = new Point(0, 283);
            punto_control_uno = new Point(112, 333);
            punto_control_dos = new Point(224, 268);
            final = new Point(300, 307);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            inicio = new Point(297, 307);
            punto_control_uno = new Point(390, 335);
            punto_control_dos = new Point(465, 290);
            final = new Point(528, 283);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            inicio = new Point(528, 283);
            punto_control_uno = new Point(680, 265);
            punto_control_dos = new Point(758, 305);
            final = new Point(860, 278);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            //Espuma
            p = new Pen(Color.White, 30);
            inicio = new Point(0, 253);
            punto_control_uno = new Point(112, 303);
            punto_control_dos = new Point(224, 238);
            final = new Point(300, 277);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            inicio = new Point(297, 277);
            punto_control_uno = new Point(390, 305);
            punto_control_dos = new Point(465, 260);
            final = new Point(528, 253);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            inicio = new Point(528, 253);
            punto_control_uno = new Point(680, 235);
            punto_control_dos = new Point(758, 275);
            final = new Point(860, 248);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            //Agua
            p = new Pen(Color.FromArgb(255, 48, 195, 215), 15);
            inicio = new Point(0, 243);
            punto_control_uno = new Point(112, 293);
            punto_control_dos = new Point(224, 228);
            final = new Point(300, 267);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            inicio = new Point(297, 267);
            punto_control_uno = new Point(390, 295);
            punto_control_dos = new Point(465, 250);
            final = new Point(528, 243);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            inicio = new Point(528, 243);
            punto_control_uno = new Point(680, 225);
            punto_control_dos = new Point(758, 265);
            final = new Point(860, 238);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            //Olas
            p = new Pen(Color.White);
            inicio = new Point(9, 213);
            final = new Point(25, 213);
            g.DrawLine(p, inicio, final);

            inicio = new Point(36, 199);
            final = new Point(44, 199);
            g.DrawLine(p, inicio, final);

            inicio = new Point(38, 218);
            final = new Point(76, 218);
            g.DrawLine(p, inicio, final);

            inicio = new Point(134, 207);
            final = new Point(143, 207);
            g.DrawLine(p, inicio, final);

            inicio = new Point(156, 199);
            final = new Point(194, 199);
            g.DrawLine(p, inicio, final);

            inicio = new Point(184, 215);
            final = new Point(197, 215);
            g.DrawLine(p, inicio, final);

            inicio = new Point(239, 210);
            final = new Point(252, 210);
            g.DrawLine(p, inicio, final);

            inicio = new Point(253, 200);
            final = new Point(268, 200);
            g.DrawLine(p, inicio, final);

            inicio = new Point(323, 201);
            final = new Point(328, 201);
            g.DrawLine(p, inicio, final);

            inicio = new Point(320, 210);
            final = new Point(358, 210);
            g.DrawLine(p, inicio, final);

            inicio = new Point(352, 216);
            final = new Point(365, 216);
            g.DrawLine(p, inicio, final);

            inicio = new Point(405, 204);
            final = new Point(418, 204);
            g.DrawLine(p, inicio, final);

            inicio = new Point(415, 197);
            final = new Point(453, 197);
            g.DrawLine(p, inicio, final);

            inicio = new Point(472, 208);
            final = new Point(489, 208);
            g.DrawLine(p, inicio, final);

            inicio = new Point(504, 200);
            final = new Point(508, 200);
            g.DrawLine(p, inicio, final);

            inicio = new Point(525, 197);
            final = new Point(542, 197);
            g.DrawLine(p, inicio, final);

            inicio = new Point(587, 199);
            final = new Point(603, 199);
            g.DrawLine(p, inicio, final);

            inicio = new Point(607, 208);
            final = new Point(612, 208);
            g.DrawLine(p, inicio, final);

            inicio = new Point(646, 196);
            final = new Point(652, 196);
            g.DrawLine(p, inicio, final);

            inicio = new Point(745, 200);
            final = new Point(762, 200);
            g.DrawLine(p, inicio, final);

            inicio = new Point(798, 203);
            final = new Point(803, 203);
            g.DrawLine(p, inicio, final);

            inicio = new Point(804, 208);
            final = new Point(818, 208);
            g.DrawLine(p, inicio, final);

            inicio = new Point(831, 197);
            final = new Point(860, 197);
            g.DrawLine(p, inicio, final);
        }

        private void DibujarBarco(PaintEventArgs e)
        {
            g = e.Graphics;
            //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias
            p = new Pen(Color.Black, 1);
            SolidBrush sb = new SolidBrush(Color.Black);
            Point[] arreglo_puntos;

            //barco
            sb = new SolidBrush(Color.Red);
            arreglo_puntos = new Point[]
            {
                new Point(705, 142),
                new Point(762, 150),
                new Point(764, 156),
                new Point(712, 155),
                new Point(705, 142)
            };
            g.FillPolygon(sb, arreglo_puntos);

            //vela del barco izquierda
            sb = new SolidBrush(Color.White);
            arreglo_puntos = new Point[]
            {
                new Point(736, 146),
                new Point(731, 145),
                new Point(729, 145),
                new Point(722, 143),
                new Point(717, 141),
                new Point(712, 139),
                new Point(710, 134),
                new Point(708, 133),
                new Point(707, 128),
                new Point(707, 120),
                new Point(708, 116),
                new Point(710, 110),
                new Point(713, 103),
                new Point(718, 94),
                new Point(719, 89),
                new Point(724, 85),
                new Point(727, 81),
                new Point(730, 75),
                new Point(736, 68),
                new Point(735, 71),
                new Point(735, 79),
                new Point(732, 95),
                new Point(733, 115),
                new Point(733, 119),
                new Point(734, 127),
                new Point(735, 133),
                new Point(736, 136),
                new Point(736, 142),
                new Point(736, 146)
            };
            g.FillPolygon(sb, arreglo_puntos);

            //vela del barco derecha
            sb = new SolidBrush(Color.White);
            arreglo_puntos = new Point[]
            {
                new Point(736, 146),
                new Point(762, 150),
                new Point(761, 142),
                new Point(761, 139),
                new Point(760, 133),
                new Point(758, 126),
                new Point(751, 104),
                new Point(745, 90),
                new Point(742, 84),
                new Point(739, 76),
                new Point(735, 68),
                new Point(737, 78),
                new Point(738, 84),
                new Point(739, 91),
                new Point(739, 99),
                new Point(740, 128),
                new Point(739, 131),
                new Point(739, 135),
                new Point(738, 137),
                new Point(738, 141),
                new Point(736, 144),
                new Point(736, 146)
            };
            g.FillPolygon(sb, arreglo_puntos);
        }

        private void DibujarIsla(PaintEventArgs e)
        {
            g = e.Graphics;
            //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias
            p = new Pen(Color.Black, 1);
            SolidBrush sb = new SolidBrush(Color.Black);
            Point[] arreglo_puntos;
            Point inicio, punto_control_uno, punto_control_dos, final;

            //isla
            sb = new SolidBrush(Color.FromArgb(255, 228, 218, 165));
            arreglo_puntos = new Point[]
            {
                new Point(40,154),
                new Point(43,151),
                new Point(47,147),
                new Point(54,144),
                new Point(51,142),
                new Point(66,141),
                new Point(71,138),
                new Point(73,138),
                new Point(76,137),
                new Point(81,138),
                new Point(86,136),
                new Point(93,136),
                new Point(112,137),
                new Point(123,138),
                new Point(127,138),
                new Point(134,141),
                new Point(140,142),
                new Point(143,143),
                new Point(146,145),
                new Point(148,149),
                new Point(151,148),
                new Point(154,151),
                new Point(156,155),
                new Point(40,154)
            };
            g.FillPolygon(sb, arreglo_puntos);

            //tronco de la palma recta
            p = new Pen(Color.Brown, 5);
            inicio = new Point(94, 137);
            final = new Point(95, 97);
            g.DrawLine(p, inicio, final);

            //hojas de la palma recta
            sb = new SolidBrush(Color.FromArgb(255, 60, 179, 99));
            arreglo_puntos = new Point[]
            {
                new Point(98, 95),
                new Point(80,111),
                new Point(78,108),
                new Point(80,99),
                new Point(84,96),
                new Point(90,93),
                new Point(89,90),
                new Point(85,87),
                new Point(78,89),
                new Point(87,81),
                new Point(91,84),
                new Point(85,89),
                new Point(90,76),
                new Point(92,71),
                new Point(97,74),
                new Point(100,83),
                new Point(106,80),
                new Point(112,84),
                new Point(109,87),
                new Point(105,87),
                new Point(102,88),
                new Point(105,93),
                new Point(108,97),
                new Point(111,108),
                new Point(107,105),
                new Point(103,102),
                new Point(95,98),
                new Point(98,95)
            };
            g.FillPolygon(sb, arreglo_puntos);

            //tronco palma curva
            p = new Pen(Color.Brown, 5);
            inicio = new Point(115, 138);
            punto_control_uno = new Point(111, 122);
            punto_control_dos = new Point(129, 77);
            final = new Point(138, 76);
            g.DrawBezier(p, inicio, punto_control_uno, punto_control_dos, final);

            //hojas del tronco crva
            sb = new SolidBrush(Color.FromArgb(255, 60, 179, 99));
            arreglo_puntos = new Point[]
            {
                new Point(130,76),
                new Point(138,75),
                new Point(136,72),
                new Point(133,71),
                new Point(132,70),
                new Point(130,71),
                new Point(125,69),
                new Point(123,68),
                new Point(124,66),
                new Point(126,66),
                new Point(128,67),
                new Point(130,65),
                new Point(139,67),
                new Point(135,66),
                new Point(137,67),
                new Point(139,69),
                new Point(139,60),
                new Point(138,59),
                new Point(139,59),
                new Point(138,54),
                new Point(137,53),
                new Point(137,51),
                new Point(138,49),
                new Point(140,53),
                new Point(143,56),
                new Point(144,60),
                new Point(144,59),
                new Point(144,67),
                new Point(145,68),
                new Point(148,67),
                new Point(150,64),
                new Point(151,64),
                new Point(151,62),
                new Point(153,62),
                new Point(154,63),
                new Point(154,62),
                new Point(154,66),
                new Point(152,67),
                new Point(151,69),
                new Point(150,70),
                new Point(148,73),
                new Point(152,73),
                new Point(152,77),
                new Point(155,77),
                new Point(157,78),
                new Point(157,79),
                new Point(157,85),
                new Point(155,84),
                new Point(154,83),
                new Point(153,81),
                new Point(148,78),
                new Point(146,77),
                new Point(145,88),
                new Point(145,90),
                new Point(144,92),
                new Point(143,95),
                new Point(142,97),
                new Point(142,95),
                new Point(141,94),
                new Point(140,91),
                new Point(140,89),
                new Point(139,75),
                new Point(138,78),
                new Point(138,75),
                new Point(130,76)
            };
            g.FillPolygon(sb, arreglo_puntos);

        }

        private void ColorearServbot()
        {
            //Color de fondo que nos ayuda a colorear
            pb_servbot.BackColor = Color.HotPink;

            //ancho y alto del picture donde se guarda el servbot
            int ancho = pb_servbot.Size.Width;
            int alto = pb_servbot.Size.Height;

            //bitmap de tamaño del picture box
            Bitmap bm = new Bitmap(ancho, alto);
            //convertimos el picturebox a bitmap
            pb_servbot.DrawToBitmap(bm, new Rectangle(0, 0, ancho, alto));
            //bm.Save(@"C:\Users\Admin\Desktop\imagen.png");

            //colereamos
            //casco
            FloodFill(bm, new Point(107, 30), Color.HotPink, Color.FromArgb(131, 141, 132));
            FloodFill(bm, new Point(135, 30), Color.HotPink, Color.FromArgb(254, 255, 164));
            FloodFill(bm, new Point(144, 30), Color.HotPink, Color.FromArgb(204, 211, 178));
            //cabeza
            FloodFill(bm, new Point(140, 60), Color.HotPink, Color.FromArgb(254, 230, 20));
            FloodFill(bm, new Point(160, 160), Color.HotPink, Color.FromArgb(233, 67, 9));
            FloodFill(bm, new Point(155, 196), Color.HotPink, Color.FromArgb(255, 174, 10));
            //torso
            FloodFill(bm, new Point(140, 240), Color.HotPink, Color.FromArgb(55, 73, 135));
            FloodFill(bm, new Point(165, 275), Color.HotPink, Color.FromArgb(63, 117, 217));
            //botones
            FloodFill(bm, new Point(135, 290), Color.HotPink, Color.FromArgb(255, 230, 23));
            FloodFill(bm, new Point(138, 307), Color.HotPink, Color.FromArgb(255, 178, 12));
            FloodFill(bm, new Point(193, 276), Color.HotPink, Color.FromArgb(255, 230, 23));
            FloodFill(bm, new Point(197, 292), Color.HotPink, Color.FromArgb(255, 178, 12));
            //piernas
            FloodFill(bm, new Point(157, 324), Color.HotPink, Color.FromArgb(96, 162, 212));
            //femur
            FloodFill(bm, new Point(116, 343), Color.HotPink, Color.FromArgb(116, 140, 142));
            FloodFill(bm, new Point(180, 335), Color.HotPink, Color.FromArgb(116, 140, 142));
            //gemelo
            FloodFill(bm, new Point(193, 382), Color.HotPink, Color.FromArgb(63, 117, 215));
            FloodFill(bm, new Point(166, 362), Color.HotPink, Color.FromArgb(55, 73, 135));
            FloodFill(bm, new Point(120, 390), Color.HotPink, Color.FromArgb(63, 117, 215));
            FloodFill(bm, new Point(100, 370), Color.HotPink, Color.FromArgb(55, 73, 135));
            //pies
            FloodFill(bm, new Point(110, 430), Color.HotPink, Color.FromArgb(123, 207, 251));
            FloodFill(bm, new Point(85, 420), Color.HotPink, Color.FromArgb(89, 159, 211));
            FloodFill(bm, new Point(115, 455), Color.HotPink, Color.FromArgb(63, 117, 217));
            FloodFill(bm, new Point(80, 440), Color.HotPink, Color.FromArgb(56, 72, 134));

            FloodFill(bm, new Point(200, 410), Color.HotPink, Color.FromArgb(123, 207, 251));
            FloodFill(bm, new Point(165, 410), Color.HotPink, Color.FromArgb(89, 159, 211));
            FloodFill(bm, new Point(210, 434), Color.HotPink, Color.FromArgb(63, 117, 217));
            FloodFill(bm, new Point(170, 430), Color.HotPink, Color.FromArgb(56, 72, 134));

            //hombro
            FloodFill(bm, new Point(55, 234), Color.HotPink, Color.FromArgb(123, 207, 251));
            FloodFill(bm, new Point(80, 230), Color.HotPink, Color.FromArgb(89, 159, 211));
            FloodFill(bm, new Point(200, 200), Color.HotPink, Color.FromArgb(123, 207, 251));
            FloodFill(bm, new Point(200, 220), Color.HotPink, Color.FromArgb(89, 159, 211));

            //codo
            FloodFill(bm, new Point(75, 265), Color.HotPink, Color.FromArgb(121, 145, 147));
            FloodFill(bm, new Point(60, 260), Color.HotPink, Color.FromArgb(194, 205, 163));
            FloodFill(bm, new Point(225, 187), Color.HotPink, Color.FromArgb(121, 145, 147));
            FloodFill(bm, new Point(215, 190), Color.HotPink, Color.FromArgb(194, 205, 163));

            //antebrazo
            FloodFill(bm, new Point(70, 300), Color.HotPink, Color.FromArgb(55, 73, 135));
            FloodFill(bm, new Point(45, 305), Color.HotPink, Color.FromArgb(60, 117, 220));
            FloodFill(bm, new Point(237, 190), Color.HotPink, Color.FromArgb(55, 73, 135));
            FloodFill(bm, new Point(237, 160), Color.HotPink, Color.FromArgb(60, 117, 220));

            //mano
            //izquierda servbot
            FloodFill(bm, new Point(63, 378), Color.HotPink, Color.FromArgb(254, 176, 18));
            FloodFill(bm, new Point(55, 340), Color.HotPink, Color.FromArgb(254, 176, 18));
            FloodFill(bm, new Point(40, 366), Color.HotPink, Color.FromArgb(254, 176, 18));
            FloodFill(bm, new Point(20, 360), Color.HotPink, Color.FromArgb(254, 176, 18));
            FloodFill(bm, new Point(43, 346), Color.HotPink, Color.FromArgb(255, 120, 28));
            FloodFill(bm, new Point(84, 366), Color.HotPink, Color.FromArgb(255, 233, 10));
            //derecha servbot
            FloodFill(bm, new Point(239, 100), Color.HotPink, Color.FromArgb(255, 233, 10));
            FloodFill(bm, new Point(256, 138), Color.HotPink, Color.FromArgb(255, 233, 10));
            FloodFill(bm, new Point(285, 137), Color.HotPink, Color.FromArgb(255, 233, 10));
            FloodFill(bm, new Point(256, 111), Color.HotPink, Color.FromArgb(254, 176, 18));
            FloodFill(bm, new Point(272, 117), Color.HotPink, Color.FromArgb(254, 176, 18));
            FloodFill(bm, new Point(272, 155), Color.HotPink, Color.FromArgb(254, 176, 18));
            FloodFill(bm, new Point(264, 102), Color.HotPink, Color.FromArgb(254, 176, 18));
            FloodFill(bm, new Point(251, 127), Color.HotPink, Color.FromArgb(255, 120, 28));

            //Quitamos el color de fondo restante de la imagen
            bm.MakeTransparent(Color.HotPink);
            //bm.Save(@"C:\Users\Admin\Desktop\imagen2.png");

            //quitamos color de fondo del picturebox y mostramos imagen
            pb_servbot.BackColor = Color.Transparent;
            pb_servbot.Image = bm;
            //pb_servbot.ImageLocation = @"C:\Users\Admin\Desktop\imagen2.png";
        }

        private void frm_principal_Load(object sender, EventArgs e)
        {

        }
    }
}
