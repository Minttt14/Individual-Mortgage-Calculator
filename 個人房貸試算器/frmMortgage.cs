using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 個人房貸試算器
{
    public partial class frmMortgage : Form
    {
        public frmMortgage()
        {
            InitializeComponent();
            txtPrice.Text = "10000000";
            txtDownPct.Text = "20";
            txtRate.Text = "2.15";
            txtYears.Text = "30";
            txtGrace.Text = "0";
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            bool isPriceValid = double.TryParse(txtPrice.Text, out double price);
            bool isDownPctValid = double.TryParse(txtDownPct.Text, out double downPct);
            bool isRateValid = double.TryParse(txtRate.Text, out double rate);
            bool isYearsValid = double.TryParse(txtYears.Text, out double years);
            bool isGraceValid = double.TryParse(txtGrace.Text, out double graceYears);

            if (isPriceValid && isDownPctValid && isRateValid && isYearsValid && isGraceValid && price > 0 && downPct >= 0 && downPct < 100 
                && rate > 0 && years > 0 && graceYears >= 0 && graceYears < years)
            {
                double loanAmount = price * (1 - (downPct / 100)); // 貸款總金額 
                double monthlyRate = (rate / 100) / 12;            // 月利率
                int totalMonths = (int)(years * 12);               // 總期數
                int graceMonths = (int)(graceYears * 12);          // 寬限期期數 

                double monthlyPayment = 0; // 每月應繳金額 
                double firstPrincipal = 0; // 首期本金 
                double firstInterest = 0;  // 首期利息 
                double totalInterest = 0;  // 總利息支出 

                // 2. 計算首期利息 (公式：貸款本金 * 月利率)
                firstInterest = loanAmount * monthlyRate;

                // 3. 判斷寬限期邏輯
                if (graceMonths > 0)
                {
                    // 寬限期內：首期不還本金
                    firstPrincipal = 0;

                    // 攤還期之每月應繳金額 (本息平均攤還公式)
                    int remainingMonths = totalMonths - graceMonths;
                    monthlyPayment = loanAmount * (monthlyRate * Math.Pow(1 + monthlyRate, remainingMonths)) /
                                     (Math.Pow(1 + monthlyRate, remainingMonths) - 1);

                    // 總利息 = (寬限期利息) + (攤還期總繳額 - 貸款本金)
                    totalInterest = (graceMonths * firstInterest) + (monthlyPayment * remainingMonths - loanAmount);
                }
                else
                {
                    // 無寬限期：標準本息平均攤還
                    monthlyPayment = loanAmount * (monthlyRate * Math.Pow(1 + monthlyRate, totalMonths)) /
                                     (Math.Pow(1 + monthlyRate, totalMonths) - 1);
                    firstPrincipal = monthlyPayment - firstInterest;
                    totalInterest = (monthlyPayment * totalMonths) - loanAmount;
                }

                //格式化輸出 (含千分位與小數點後兩位 "N2")
                txtResultLoan.Text = loanAmount.ToString("N2");
                txtResultPrincipal.Text = firstPrincipal.ToString("N2");
                txtResultInterest.Text = firstInterest.ToString("N2");
                txtResultMonthlyPay.Text = monthlyPayment.ToString("N2");
                txtResultTotalInterest.Text = totalInterest.ToString("N2");
                txtResultTotalRepay.Text = (loanAmount + totalInterest).ToString("N2");
            }
            else
            {
                string errorMsg = "請輸入有效的數字，並確保：\n" +
                                  "1. 房屋總價與利率大於 0\n" +
                                  "2. 自備款比例介於 0 到 100 之間\n" +
                                  "3. 寬限期必須小於貸款年限";

                MessageBox.Show(errorMsg, "輸入驗證失敗", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtPrice.Clear();
            txtDownPct.Clear();
            txtRate.Clear();
            txtYears.Clear();
            txtGrace.Clear();
            txtResultLoan.Text = "";
            txtResultPrincipal.Text = "";
            txtResultInterest.Text = "";
            txtResultMonthlyPay.Text = "";
            txtResultTotalInterest.Text = "";
            txtResultTotalRepay.Text = "";
            txtPrice.Focus(); // 焦點回到第一個欄位
        }
    }
}
