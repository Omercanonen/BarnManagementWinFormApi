namespace Client.Pages
{
    partial class PurchaseWorker
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            listBoxWorkers = new ListBox();
            buttonBuyWorker = new Button();
            groupBox1 = new GroupBox();
            buttonSell = new Button();
            buttonUpgrade = new Button();
            labelInfo = new Label();
            labelWorkerPrice = new Label();
            labelUpgrade = new Label();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // listBoxWorkers
            // 
            listBoxWorkers.FormattingEnabled = true;
            listBoxWorkers.Location = new Point(3, 3);
            listBoxWorkers.Name = "listBoxWorkers";
            listBoxWorkers.Size = new Size(187, 424);
            listBoxWorkers.TabIndex = 0;
            // 
            // buttonBuyWorker
            // 
            buttonBuyWorker.Font = new Font("Segoe UI", 16F);
            buttonBuyWorker.Location = new Point(283, 48);
            buttonBuyWorker.Name = "buttonBuyWorker";
            buttonBuyWorker.Size = new Size(191, 65);
            buttonBuyWorker.TabIndex = 1;
            buttonBuyWorker.Text = "Buy Worker";
            buttonBuyWorker.UseVisualStyleBackColor = true;
            buttonBuyWorker.Click += buttonBuyWorker_Click;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(labelUpgrade);
            groupBox1.Controls.Add(buttonSell);
            groupBox1.Controls.Add(buttonUpgrade);
            groupBox1.Controls.Add(labelInfo);
            groupBox1.Location = new Point(207, 165);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(360, 241);
            groupBox1.TabIndex = 2;
            groupBox1.TabStop = false;
            groupBox1.Text = "groupBox1";
            // 
            // buttonSell
            // 
            buttonSell.Font = new Font("Segoe UI", 14F);
            buttonSell.Location = new Point(234, 175);
            buttonSell.Name = "buttonSell";
            buttonSell.Size = new Size(104, 45);
            buttonSell.TabIndex = 2;
            buttonSell.Text = "Sell";
            buttonSell.UseVisualStyleBackColor = true;
            buttonSell.Click += buttonSell_Click;
            // 
            // buttonUpgrade
            // 
            buttonUpgrade.Font = new Font("Segoe UI", 14F);
            buttonUpgrade.Location = new Point(16, 173);
            buttonUpgrade.Name = "buttonUpgrade";
            buttonUpgrade.Size = new Size(106, 47);
            buttonUpgrade.TabIndex = 1;
            buttonUpgrade.Text = "Upgrade";
            buttonUpgrade.UseVisualStyleBackColor = true;
            buttonUpgrade.Click += buttonUpgrade_Click;
            // 
            // labelInfo
            // 
            labelInfo.AutoSize = true;
            labelInfo.Font = new Font("Segoe UI", 14F);
            labelInfo.Location = new Point(16, 37);
            labelInfo.Name = "labelInfo";
            labelInfo.Size = new Size(63, 25);
            labelInfo.TabIndex = 0;
            labelInfo.Text = "label1";
            // 
            // labelWorkerPrice
            // 
            labelWorkerPrice.AutoSize = true;
            labelWorkerPrice.Font = new Font("Segoe UI", 14F);
            labelWorkerPrice.Location = new Point(283, 20);
            labelWorkerPrice.Name = "labelWorkerPrice";
            labelWorkerPrice.Size = new Size(63, 25);
            labelWorkerPrice.TabIndex = 3;
            labelWorkerPrice.Text = "label1";
            // 
            // labelUpgrade
            // 
            labelUpgrade.AutoSize = true;
            labelUpgrade.Location = new Point(16, 155);
            labelUpgrade.Name = "labelUpgrade";
            labelUpgrade.Size = new Size(38, 15);
            labelUpgrade.TabIndex = 3;
            labelUpgrade.Text = "label1";
            // 
            // PurchaseWorker
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(labelWorkerPrice);
            Controls.Add(groupBox1);
            Controls.Add(buttonBuyWorker);
            Controls.Add(listBoxWorkers);
            Name = "PurchaseWorker";
            Size = new Size(570, 426);
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListBox listBoxWorkers;
        private Button buttonBuyWorker;
        private GroupBox groupBox1;
        private Button buttonSell;
        private Button buttonUpgrade;
        private Label labelInfo;
        private Label labelWorkerPrice;
        private Label labelUpgrade;
    }
}
