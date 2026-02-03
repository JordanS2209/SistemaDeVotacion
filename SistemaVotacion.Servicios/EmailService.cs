using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;

namespace SistemaVotacion.Servicios
{
    public class EmailService
    {
        public async Task EnviarCorreoRegistro(string emailDestino, string nombreUsuario)
        {
            var mensaje = new MimeMessage();
            mensaje.From.Add(new MailboxAddress("Sistema AutomaG", "josuem3693@gmail.com"));
            mensaje.To.Add(new MailboxAddress(nombreUsuario, emailDestino));
            mensaje.Subject = "¡Bienvenido al Sistema!";

            mensaje.Body = new TextPart("html")
            {
                Text = $@"
                <h1>Hola {nombreUsuario},</h1>
                <p>Tu registro se ha completado con éxito.</p>
                <p>Ya puedes acceder al sistema con tus credenciales.</p>
                <br>
                <p>Saludos,<br>El equipo de AutomaG</p>"
            };

            using (var cliente = new SmtpClient())
            {
                // Para pruebas rápidas, te recomiendo usar Mailtrap.io
                // Si usas Gmail, recuerda que necesitas una 'Contraseña de Aplicación'
                await cliente.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                await cliente.AuthenticateAsync("josuem3693@gmail.com", "mnrcagnrbzvzjxur");
                await cliente.SendAsync(mensaje);
                await cliente.DisconnectAsync(true);
            }
        }
    }
}
