using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Avialines
{
    public class BaseForm : Form
    {
        public BaseForm()
        {
            // Устанавливаем фиксированное положение и размер для всех форм
            this.StartPosition = FormStartPosition.Manual;  // Ручная установка позиции
            this.Location = new Point(200, 100);  // Позиция на экране (можешь изменить координаты)
            this.Size = new Size(800, 600);  // Фиксированный размер (можешь изменить размеры)

            // Заблокировать изменение размера формы
            this.FormBorderStyle = FormBorderStyle.FixedDialog;  // Фиксированный размер формы
            this.MaximizeBox = false;  // Отключаем кнопку максимизации
            this.MinimizeBox = true;  // Оставляем кнопку минимизации
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // BaseForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "BaseForm";
            this.Load += new System.EventHandler(this.BaseForm_Load);
            this.ResumeLayout(false);

        }

        private void BaseForm_Load(object sender, EventArgs e)
        {

        }
    }

}
