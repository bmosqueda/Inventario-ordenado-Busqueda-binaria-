using System;
using System.Windows.Forms;

namespace Inventario
{
    public partial class Form1 : Form
    {
        public class Product
        {
            private int id;
            private string nombre;
            private double costo;

            public int Id { get { return id; } }
            public string Nombre { get { return nombre; } }
            public double Costo { get { return costo; } }

            public string Descripcion { get; set; }
            public int Cantidad { get; set; }

            public Product(int id, string name, double cost, string description, int amount)
            {
                this.id = id;
                this.nombre = name;
                this.costo = cost;
                this.Descripcion = description;
                this.Cantidad = amount;
            }

            public Product()
            {
                this.id = 0;
            }

            public Product(int id)
            {
                this.id = id;
            }

            public override string ToString()
            {
                return "******* " + nombre + " *******\n" +
                        "Código: " + id + "\n" +
                        "Cantidad: " + Cantidad + "\n" +
                        "Costo: " + costo + "\n\n" +
                        "Descripción: " + Descripcion;
            }
        }

        public class Inventory
        {
            public Product[] products;
            public int Length { get { return products.Length; } }
            private int lastElement;

            public Inventory(int productsNumber)
            {
                //Si el primer elemento que se agrega tiene un id mayor al número de elementos + 1 
                //hay un pequeño problrma, no se truena pero no lo graba.
                products = new Product[productsNumber];
                for (int i = 0; i < Length; i++)
                {
                    products[i] = new Product(productsNumber + 1);
                }
                lastElement = 0;
            }

            //retun the position of the found product or -1
            public Product search(int id)
            {
                int min = 0;
                int max = lastElement - 2;
                int mean;

                while (max >= min)
                {
                    mean = (min + max) / 2;
                    if (products[mean].Id == id)
                        return products[mean];

                    if (id > mean)
                        min = mean + 1;
                    else
                        max = mean - 1;
                }
                return null;
            }

            //Returns the position in the array products where it must be this id or -1 if this id already exists
            public int getInsertPosition(int id)
            {
                int min = 0;
                //-2 because last element save the number of elements + 1 and because the first element in the array is 0
                int max = lastElement - 2 < 0 ? 0 : lastElement - 1;
                int mean;
                int position = 0;

                while (max >= min)
                {
                    mean = (min + max) / 2;
                    if (products[mean].Id == id)
                        return -1;

                    if (id > products[mean].Id)
                        min = mean + 1;
                    else
                        max = mean - 1;

                    position = mean;
                }

                //Problem
                position = products[position].Id > id ? position : position + 1;
                return position;
            }

            public string add(int id, string name, double cost, string description, int amount)
            {
                if (lastElement < Length)
                {
                    int insertPosition = getInsertPosition(id);
                    if (insertPosition != -1)
                    {
                        insert(insertPosition, new Product(id, name, cost, description, amount));
                        lastElement++;
                        return "Se agregó exitosamente el producto";
                    }
                    else
                        return "Ya existe un producto con el id " + id;
                }
                else
                {
                    return "Ya no se pueden agregar más products";
                }
            }

            public void insert(int position, Product product)
            {
                //i <= lastElement because lastElement must be incremented int after of this method no before
                for (int i = position; i <= lastElement; i++)
                {
                    Product temp = new Product(products[i].Id, products[i].Nombre, products[i].Costo, products[i].Descripcion, products[i].Cantidad);
                    products[i] = new Product(product.Id, product.Nombre, product.Costo, product.Descripcion, product.Cantidad);
                    product = temp;
                }
            }

            public string delete(int pos)
            {
                if (pos < 0)
                {
                    return "La posición debe ser mayor a 0";
                }
                else if (pos > lastElement)
                {
                    return "La posición debe ser menor a la cantidad actual de products (" + lastElement + ")";
                }
                else
                {
                    for (int i = pos; i < lastElement - 1; i++)
                        products[i] = new Product(products[i + 1].Id, products[i + 1].Nombre, products[i + 1].Costo, products[i + 1].Descripcion, products[i + 1].Cantidad);

                    products[lastElement - 1] = new Product(Length + 1);

                    lastElement--;
                    return "Se eliminó correctamente el producto";
                }
            }

            public int searchPosition(int id)
            {
                int min = 0;
                int max = lastElement - 2;
                int mean;

                while (max >= min)
                {
                    mean = (min + max) / 2;
                    if (products[mean].Id == id)
                        return mean;

                    if (id > mean)
                        min = mean + 1;
                    else
                        max = mean - 1;
                }
                return - 1;
            }

            public string deleteById(int id)
            {
                int deleteProductPosition = searchPosition(id);
                if (deleteProductPosition != -1)
                {
                    return delete(deleteProductPosition);
                }
                else
                    return "El producto con el id " + id + " no se puede eliminar porque no existe";
            }

            public string listProducts()
            {
                string productsString = "";
                for (int i = 0; i < lastElement; i++)
                {
                    productsString += products[i].ToString() + "\n__________________________________\n";
                }
                return productsString;
            }
        }

        Inventory inventory;

       public Form1()
        {
            InitializeComponent();
            inventory = new Inventory(15);
            //addProducts();
        }

        //Debuggin method
        public void addProducts()
        {
            for (int i = 1; i < 10; i++)
            {
                inventory.add(i * 2, "producto " + (i * 2), i * 2, "description " + (i*2) + " " + (i*2) + " " + " " + (i*2), (i*2));
            }
        }

        private void btnListar_Click(object sender, EventArgs e)
        {
            txtMostrar.Text = inventory.listProducts();
            lblEstado.Text = "Elementos listados";
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (txtNombre.Text.Trim() != "" && txtDescripcion.Text.Trim() != "")
            {
                string agregar = inventory.add(Convert.ToInt32(numCodigo.Value), txtNombre.Text, Convert.ToDouble(numCantidad.Value), txtDescripcion.Text, Convert.ToInt32(numCantidad.Value));
                btnListar.PerformClick();
                lblEstado.Text = agregar;
            }
        }

        private void btnElimar_Click(object sender, EventArgs e)
        {
            int posicion = Convert.ToInt32(numPosicion.Value);
            string eliminar = inventory.delete(posicion);
            btnListar.PerformClick();
            lblEstado.Text = eliminar;
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            int codigo = Convert.ToInt32(numCodigo.Value);
            Product buscar = inventory.search(codigo);
            if (buscar != null)
            {
                lblEstado.Text = "Producto encontrado";
                MessageBox.Show(buscar.ToString());
            }
            else
            {
                lblEstado.Text = "Product no encontrado";
            }
        }

        private void btnEliminarCodigo_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(numCodigo.Value);
            string eliminar = inventory.deleteById(id);
            btnListar.PerformClick();
            lblEstado.Text = eliminar;
        }
    }
}