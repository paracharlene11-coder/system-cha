using System.ComponentModel;
using System.Text;

namespace FoodOrdering.Cashier;

public class MainForm : Form
{
    private readonly ApiClient _api = new();
    private readonly BindingList<Order> _orders = new();

    private readonly DataGridView _grid = new();
    private readonly TextBox _details = new();
    private readonly ComboBox _status = new();
    private readonly Button _refresh = new();
    private readonly Button _update = new();
    private readonly Button _print = new();

    private readonly TextBox _amountPaid = new();
    private readonly Label _changeLabel = new();
    private readonly Button _pay = new();

    public MainForm()
    {
        Text = "Food Ordering - Cashier/Admin Dashboard";
        Width = 1250;
        Height = 720;
        MinimumSize = new Size(1100, 600);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.FromArgb(18, 18, 18);
        ForeColor = Color.White;

        BuildLayout();

        Shown += async (_, _) => await LoadOrders();
    }

    private void BuildLayout()
    {
        var main = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 2,
            ColumnCount = 1,
            BackColor = Color.FromArgb(18, 18, 18)
        };

        main.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));
        main.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        Controls.Add(main);

        var title = new Label
        {
            Text = "Cashier Dashboard",
            Font = new Font("Segoe UI", 22, FontStyle.Bold),
            Dock = DockStyle.Fill,
            ForeColor = Color.White,
            BackColor = Color.FromArgb(18, 18, 18),
            Padding = new Padding(16, 15, 0, 0)
        };

        main.Controls.Add(title, 0, 0);

        var split = new SplitContainer
        {
            Dock = DockStyle.Fill,
            SplitterDistance = 870,
            BackColor = Color.FromArgb(18, 18, 18),
            BorderStyle = BorderStyle.FixedSingle
        };

        main.Controls.Add(split, 0, 1);

        BuildLeftPanel(split.Panel1);
        BuildRightPanel(split.Panel2);
    }

    private void BuildLeftPanel(Control parent)
    {
        var left = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 3,
            ColumnCount = 1,
            BackColor = Color.FromArgb(18, 18, 18),
            Padding = new Padding(12)
        };

        left.RowStyles.Add(new RowStyle(SizeType.Absolute, 45));
        left.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));
        left.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        parent.Controls.Add(left);

        var sectionTitle = new Label
        {
            Text = "Current Orders",
            Font = new Font("Segoe UI", 15, FontStyle.Bold),
            Dock = DockStyle.Fill,
            ForeColor = Color.White,
            Padding = new Padding(0, 5, 0, 0)
        };

        left.Controls.Add(sectionTitle, 0, 0);

        var toolbar = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(0, 8, 0, 8),
            BackColor = Color.FromArgb(30, 30, 30),
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            AutoScroll = true
        };

        _refresh.Text = "Refresh Orders";
        _refresh.Width = 130;
        _refresh.Height = 32;
        _refresh.Click += async (_, _) => await LoadOrders();

        _status.Items.AddRange(new object[]
        {
            "PREPARING",
            "PENDING",
            "COMPLETED",
            "CANCELLED"
        });
        _status.SelectedIndex = 0;
        _status.Width = 135;
        _status.Height = 32;
        _status.DropDownStyle = ComboBoxStyle.DropDownList;

        _update.Text = "Update Status";
        _update.Width = 130;
        _update.Height = 32;
        _update.Click += async (_, _) => await UpdateSelectedStatus();

        _amountPaid.Width = 120;
        _amountPaid.Height = 32;
        _amountPaid.PlaceholderText = "Amount paid";

        _pay.Text = "Accept Payment";
        _pay.Width = 130;
        _pay.Height = 32;
        _pay.Click += async (_, _) => await AcceptPayment();

        _changeLabel.Text = "Change: $0.00";
        _changeLabel.Width = 150;
        _changeLabel.Height = 32;
        _changeLabel.ForeColor = Color.White;
        _changeLabel.TextAlign = ContentAlignment.MiddleLeft;

        _print.Text = "Print Receipt";
        _print.Width = 120;
        _print.Height = 32;
        _print.Click += async (_, _) => await PrintReceipt();

        toolbar.Controls.Add(_refresh);
        toolbar.Controls.Add(_status);
        toolbar.Controls.Add(_update);
        toolbar.Controls.Add(_amountPaid);
        toolbar.Controls.Add(_pay);
        toolbar.Controls.Add(_changeLabel);
        toolbar.Controls.Add(_print);

        left.Controls.Add(toolbar, 0, 1);

        SetupGrid();
        left.Controls.Add(_grid, 0, 2);
    }

    private void SetupGrid()
    {
        _grid.Dock = DockStyle.Fill;
        _grid.AutoGenerateColumns = false;
        _grid.AllowUserToAddRows = false;
        _grid.AllowUserToDeleteRows = false;
        _grid.ReadOnly = true;
        _grid.MultiSelect = false;
        _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _grid.RowHeadersVisible = false;
        _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _grid.BackgroundColor = Color.FromArgb(24, 24, 24);
        _grid.BorderStyle = BorderStyle.None;
        _grid.EnableHeadersVisualStyles = false;

        _grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(45, 45, 45);
        _grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        _grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
        _grid.ColumnHeadersHeight = 36;

        _grid.DefaultCellStyle.BackColor = Color.FromArgb(32, 32, 32);
        _grid.DefaultCellStyle.ForeColor = Color.White;
        _grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(230, 145, 40);
        _grid.DefaultCellStyle.SelectionForeColor = Color.Black;
        _grid.DefaultCellStyle.Font = new Font("Segoe UI", 10);
        _grid.RowTemplate.Height = 34;

        _grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(38, 38, 38);
        _grid.GridColor = Color.FromArgb(60, 60, 60);

        _grid.Columns.Clear();

        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Order ID",
            DataPropertyName = "OrderCode",
            FillWeight = 140
        });

        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Customer",
            DataPropertyName = "CustomerName",
            FillWeight = 160
        });

        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Total",
            DataPropertyName = "TotalAmount",
            FillWeight = 100,
            DefaultCellStyle = new DataGridViewCellStyle
            {
                Format = "C2",
                ForeColor = Color.White,
                BackColor = Color.FromArgb(32, 32, 32),
                SelectionBackColor = Color.FromArgb(230, 145, 40),
                SelectionForeColor = Color.Black
            }
        });

        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Status",
            DataPropertyName = "Status",
            FillWeight = 120
        });

        _grid.DataSource = _orders;
        _grid.SelectionChanged += (_, _) => ShowSelectedDetails();
    }

    private void BuildRightPanel(Control parent)
    {
        var right = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 2,
            ColumnCount = 1,
            BackColor = Color.FromArgb(18, 18, 18),
            Padding = new Padding(12)
        };

        right.RowStyles.Add(new RowStyle(SizeType.Absolute, 45));
        right.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        parent.Controls.Add(right);

        var receiptTitle = new Label
        {
            Text = "Receipt Preview",
            Font = new Font("Segoe UI", 15, FontStyle.Bold),
            Dock = DockStyle.Fill,
            ForeColor = Color.White,
            Padding = new Padding(0, 5, 0, 0)
        };

        right.Controls.Add(receiptTitle, 0, 0);

        _details.Dock = DockStyle.Fill;
        _details.Multiline = true;
        _details.ReadOnly = true;
        _details.ScrollBars = ScrollBars.Vertical;
        _details.Font = new Font("Consolas", 10);
        _details.BackColor = Color.FromArgb(250, 246, 239);
        _details.ForeColor = Color.FromArgb(30, 30, 30);
        _details.BorderStyle = BorderStyle.FixedSingle;
        _details.Text = "Select an order to view receipt.";

        right.Controls.Add(_details, 0, 1);
    }

    private async Task LoadOrders()
    {
        try
        {
            _orders.Clear();

            var orders = await _api.GetOrdersAsync();

            foreach (var order in orders)
            {
                _orders.Add(order);
            }

            if (_orders.Count > 0)
            {
                _grid.ClearSelection();
                _grid.Rows[0].Selected = true;
                _grid.CurrentCell = _grid.Rows[0].Cells[0];
                ShowSelectedDetails();
            }
            else
            {
                _details.Text = "No orders yet.\r\n\r\nPlace an order from the web app first.";
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Cannot connect to API.\n\nStart Java backend first:\nhttp://localhost:8080\n\n{ex.Message}",
                "Connection Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }
    }

    private Order? SelectedOrder
    {
        get
        {
            if (_grid.CurrentRow?.DataBoundItem is Order order)
            {
                return order;
            }

            return null;
        }
    }

    private void ShowSelectedDetails()
    {
        var order = SelectedOrder;

        if (order == null)
        {
            return;
        }

        var sb = new StringBuilder();

        sb.AppendLine("Charlie's Food");
        sb.AppendLine("==============================");
        sb.AppendLine($"Order: {order.OrderCode}");
        sb.AppendLine($"Customer: {order.CustomerName}");
        sb.AppendLine($"Phone: {order.CustomerPhone}");
        sb.AppendLine($"Address: {order.DeliveryAddress}");
        sb.AppendLine($"Order Status: {order.Status}");
        sb.AppendLine($"Payment Method: {order.PaymentMethod}");
        sb.AppendLine($"Payment Status: {order.PaymentStatus}");

        if (order.PaymentStatus == "PAID")
        {
            sb.AppendLine($"Amount Paid: {order.AmountPaid:C}");
            sb.AppendLine($"Change: {order.ChangeAmount:C}");
        }

        sb.AppendLine();
        sb.AppendLine("Items");

        foreach (var item in order.Items)
        {
            sb.AppendLine($"- {item.ItemName} x {item.Quantity} @ {item.UnitPrice:C} = {item.LineTotal:C}");
        }

        sb.AppendLine();
        sb.AppendLine($"TOTAL: {order.TotalAmount:C}");

        _details.Text = sb.ToString();

        if (_status.Items.Contains(order.Status))
        {
            _status.SelectedItem = order.Status;
        }

        _changeLabel.Text = $"Change: {order.ChangeAmount:C}";

        if (order.AmountPaid > 0)
        {
            _amountPaid.Text = order.AmountPaid.ToString("0.00");
        }
        else
        {
            _amountPaid.Clear();
        }
    }

    private async Task UpdateSelectedStatus()
    {
        var order = SelectedOrder;

        if (order == null)
        {
            MessageBox.Show("Please select an order first.", "No Order Selected");
            return;
        }

        try
        {
            var newStatus = _status.SelectedItem?.ToString() ?? "PREPARING";
            var updated = await _api.UpdateStatusAsync(order.Id, newStatus);

            if (updated != null)
            {
                var idx = _orders.IndexOf(order);

                if (idx >= 0)
                {
                    _orders[idx] = updated;
                    _grid.ClearSelection();
                    _grid.Rows[idx].Selected = true;
                    _grid.CurrentCell = _grid.Rows[idx].Cells[0];
                    ShowSelectedDetails();
                }

                MessageBox.Show("Order status updated.", "Success");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Update Failed");
        }
    }

    private async Task AcceptPayment()
    {
        var order = SelectedOrder;

        if (order == null)
        {
            MessageBox.Show("Please select an order first.", "No Order Selected");
            return;
        }

        if (!decimal.TryParse(_amountPaid.Text, out var amountPaid))
        {
            MessageBox.Show("Please enter a valid payment amount.", "Invalid Amount");
            return;
        }

        if (amountPaid < order.TotalAmount)
        {
            MessageBox.Show(
                $"Insufficient payment.\n\nTotal: {order.TotalAmount:C}\nAmount Paid: {amountPaid:C}",
                "Payment Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );
            return;
        }

        var change = amountPaid - order.TotalAmount;
        _changeLabel.Text = $"Change: {change:C}";

        try
        {
            var updated = await _api.UpdatePaymentAsync(order.Id, amountPaid);

            if (updated != null)
            {
                var idx = _orders.IndexOf(order);

                if (idx >= 0)
                {
                    _orders[idx] = updated;
                    _grid.ClearSelection();
                    _grid.Rows[idx].Selected = true;
                    _grid.CurrentCell = _grid.Rows[idx].Cells[0];
                    ShowSelectedDetails();
                }

                _changeLabel.Text = $"Change: {change:C}";

                MessageBox.Show(
                    $"Payment accepted.\n\nTotal: {order.TotalAmount:C}\nAmount Paid: {amountPaid:C}\nChange: {change:C}",
                    "Payment Successful",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Payment Failed");
        }
    }

    private async Task PrintReceipt()
    {
        var order = SelectedOrder;

        if (order == null)
        {
            MessageBox.Show("Please select an order first.", "No Order Selected");
            return;
        }

        try
        {
            var receipt = await _api.GetReceiptAsync(order.Id);

            if (receipt == null)
            {
                MessageBox.Show("Receipt was not found.", "Receipt Failed");
                return;
            }

            var path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                $"receipt-{order.OrderCode}.txt"
            );

            File.WriteAllText(
                path,
                _details.Text +
                Environment.NewLine +
                $"Receipt No: {receipt.ReceiptNumber}" +
                Environment.NewLine +
                $"Generated: {receipt.GeneratedAt}"
            );

            MessageBox.Show($"Receipt saved to Desktop:\n{path}", "Receipt Printed");
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Receipt Failed");
        }
    }
}