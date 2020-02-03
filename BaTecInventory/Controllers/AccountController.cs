using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using BaTecInventory.Models;
using PagedList;
using System.IO;
using System.Web.UI;



namespace BaTecInventory.Controllers
{
    
    public class AccountController : Controller
    {

        DB.DBConnect _db = new DB.DBConnect();
        private List<List<string>> tempData;
        private List<Product> Product;
        private List<User> userlList = null;
        private List<Product> returnList;

        public AccountController()
        {
            userlList = _db.Users();
            _db = new DB.DBConnect();

            tempData = new List<List<string>>();
            string query =
                "SELECT " +
                "`ProductId`, " +
                "`SerialNumber`, " +
                "`ReceiptNumber`, " +
                "`PurchaseDate`, " +
                "`DestructionDate`, " +
                "`Comments`, " +
                "`History`, " +
                "`User_Id`, " +
                "`Model_Id`, " +
                "`Supplier_Id`, " +
                "`SubCategory_Id` " +
                "FROM inventory.product;";

            tempData = _db.Show(query);

            Product = new List<Product>();



            int productID = 0;
            int subCategoryID = 0;
            int modelID = 0;
            int userID = 0;
            int supplierID = 0;

            foreach (List<string> x in tempData)
            {
                Product prod = new Product
                {
                    Model = _db.Select("SELECT modelname FROM inventory.model WHERE modelid = " + x[8]),
                    SubCategory = _db.Select("SELECT subcategoryname FROM inventory.subcategory WHERE SubCategoryId = " + x[10]),
                    Supplier = _db.Select("SELECT suppliername FROM inventory.supplier WHERE SupplierId = " + x[9]),
                    UserName = _db.Select("SELECT username FROM inventory.user WHERE UserId = " + x[7]),
                    Category = _db.Select("SELECT cat.categoryname FROM inventory.category AS cat INNER JOIN inventory.SubCategory AS sub ON sub.Category_Id = cat.CategoryId WHERE sub.SubCategoryId = '" + x[10] + "';"),
                    Producer = _db.Select("SELECT pro.producername FROM inventory.producer AS pro INNER JOIN inventory.model AS mo ON mo.Producer_Id = pro.ProducerId WHERE mo.modelid = '" + x[8] + "';"),
                    ProductId = int.Parse(x[0]),
                    SerialNumber = x[1],
                    ReceiptNumber = int.Parse(x[2]),
                    PurchaseDate = DateTime.Parse(x[3]),
                    DestructionDate = DateTime.Parse(x[4]),
                    Comments = x[5],
                    History = x[6]
                };

                productID = int.Parse(x[0]);
                subCategoryID = int.Parse(x[10]);
                modelID = int.Parse(x[8]);
                userID = int.Parse(x[7]);
                supplierID = int.Parse(x[9]);


                Product.Add(prod);


            }

        }

        // GET: Account
       
        public ActionResult Index(int? page)
        {
            string username = this.ControllerContext.HttpContext.Request.Cookies["Username"].Value;

            returnList = new List<Product>();
            

            foreach (Product temp in Product)
            {
                if (temp.UserName == username)
                    returnList.Add(temp);
            }

            Session["returnList"] = returnList.ToList();
            return View(returnList.ToPagedList(page ?? 1, 3));
        }

        [HttpGet]
        public void ExportToExcel()
        {
            returnList = (List<Product>)Session["returnList"];

            Helper.ExcelManager.ExportExcel();
            var gv = new GridView
            {
                DataSource = returnList
            };
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=inventory_list.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = " ";
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
            gv.AllowCustomPaging = false;
            gv.AllowPaging = true;
            gv.RenderControl(objHtmlTextWriter);
            Response.Output.Write(objStringWriter.ToString());
            Response.Flush();
            Response.End();

        }
      
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
      
        public ActionResult Login(string username, string password)
        {
            if ("admin".Equals(username) && "123".Equals(password))
            {
                //Session["user"] = new User() { UserName = username, Password = "123" };
                return RedirectToAction("Index", "Home");
            }

            try
            {
                if (ModelState.IsValid)
                {
                    var user = userlList.Find(x => x.UserName.Equals(username));
                    if (user != null)
                    {
                        user = userlList.Find(x => x.Password.Equals(password));
                        if (user != null)
                        {
                            HttpCookie cookie = new HttpCookie("Username")
                            {
                                Value = username
                            };

                            this.ControllerContext.HttpContext.Response.Cookies.Add(cookie);

                           
                            return RedirectToAction("Index", "Account");
                        }
                        else
                        {
                            
                            ViewBag.Message = "PLEASE VERIFY YOUR PASSWORD";
                            return View();
                        }

                    }
                    else
                    {
                       
                        ViewBag.Message = "THERE IS NO USER REGISTERED WITH THIS USER NAME ";
                        return View();
                    }

                }
            }
            catch (Exception)
            {
                
                ViewBag.Message = "THERE IS NO USER REGISTERED WITH THIS USER NAME ";
                return View();
            }



            return View();
        }

        public ActionResult Logout()
        {
            this.ControllerContext.HttpContext.Response.Cookies.Clear();
            //Session.Clear();
            //or Session ["user"] = null;
            RedirectToAction("Login", "Account");
            return View();
        }
        // GET: Account/Details/5
        public ActionResult Details(int? id)
        {
            var pdt = Product.Find(m => m.ProductId == id);
            return View(pdt);
        }



        // GET: Account/Edit/5
        public ActionResult Edit(int? id)
        {
            var pdt = Product.Find(m => m.ProductId == id);

            return View(pdt);
        }

       // POST: Account/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Product product)
        {
            int supplierid = FindOrCreateSupplier(product.Supplier);
            if (supplierid == 0)
            {
                return View();
            }
            int categoryid = FindOrCreateCategory(product.Category);
            if (categoryid == 0)
            {
                return View();
            }
            int producerid = FindOrCreateProducer(product.Producer);
            if (producerid == 0)
            {
                return View();
            }
            //int userid = FindOrCreateUser(product.UserName);
            //if (userid == 0)
            //{
            //    return View();
            //}

            //user_id = ('" + userid + "'),

            int subcategoryid = FindOrCreateSubCategory(product.SubCategory, categoryid);
            if (subcategoryid == 0)
            {
                return View();
            }
            int modelid = FindOrCreateModel(product.Model, producerid);
            if (modelid == 0)
            {
                return View();
            }

            var pdt = Product.Find(m => m.ProductId == id);

            if (id == pdt.ProductId)
            {
                var query = "UPDATE inventory.Product Set SerialNumber = ('" + product.SerialNumber + "'),ReceiptNumber=('" + product.ReceiptNumber + "'),PurchaseDate=('" + product.PurchaseDate.ToString("yyyy-MM-dd") + "'),DestructionDate=('" + product.DestructionDate.ToString("yyyy-MM-dd") + "'),Comments=('" + product.Comments + "'),History=('" + product.History + "')" +
                            ",  model_id = ('" + modelid + "'),supplier_id = ('" + supplierid + "'), subcategory_id = ('" + subcategoryid + "') Where ProductId = ('" + pdt.ProductId + "')";
                _db.Edit(query);

                return RedirectToAction("Index");
            }


            return View(product);
        }

        private int FindOrCreateSupplier(string supplierName)
        {
            int id = 0;
            try
            {
                string queryFindId = "SELECT supplierId FROM inventory.supplier WHERE SupplierName = '" + supplierName.ToLower() + "' LIMIT 1;";
                // connection to the databae get the ID 
                id = _db.FindID(queryFindId, "SupplierId");
                if (id > -1)
                {
                    return id;
                }

                /********************************************************************************************************************************************************************/
                /*  BOX 2
                 *  + validate the name
                 *  + [INSERT STEP]
                 *      + Create query to the database
                 *      - Connect to the database 
                 *          - Execute the query we created thrue the database
                 *      - If it is correst
                 *  - Create the suppliername on the database
                 *  - Connect to the database
                 *  
                 */
                string QueryCreateSupplier = "INSERT INTO inventory.supplier(suppliername) VALUES ('" + supplierName.ToLower() + "')";
                _db.Create(QueryCreateSupplier);


                // connect to the database and create the suppliername

                /********************************************************************************************************************************************************************/
                // BOX 3

                //// try to find the id again
                queryFindId = "SELECT supplierid FROM inventory.supplier WHERE suppliername = '" + supplierName.ToLower() + "'LIMIT 1;";
                //// connection to the databae get the ID 
                int returnvaluefromthedatabase = _db.FindID(queryFindId, "SupplierId");
                if (returnvaluefromthedatabase > 0)
                {
                    return returnvaluefromthedatabase;
                }



                /********************************************************************************************************************************************************************/

            }
            catch
            {
                id = 0;
            }


            return id;
        }

        private int FindOrCreateCategory(string categoryName)
        {
            int id = 0;
            try
            {
                string QueryFindID = "SELECT categoryId FROM inventory.category WHERE CategoryName = '" + categoryName.ToLower() + "' LIMIT 1;";

                id = _db.FindID(QueryFindID, "CategoryId");
                if (id > -1)
                    return id;


                string QueryCreateCategory = "INSERT INTO inventory.category(categoryname) VALUES ('" + categoryName.ToLower() + "')";
                _db.Create(QueryCreateCategory);
                // connect to the database and create the categoryname

                //// try to find the id again
                QueryFindID = "SELECT categoryId FROM inventory.category WHERE CategoryName = '" + categoryName.ToLower() + "' LIMIT 1;";
                //// connection to the databae get the ID 
                int returnvaluefromthedatabase = _db.FindID(QueryFindID, "CategoryId");
                if (returnvaluefromthedatabase > 0)
                {
                    return returnvaluefromthedatabase;
                }



            }
            catch
            {
                id = 0;
            }


            return id;
        }

        private int FindOrCreateProducer(string producerName)
        {
            int id = 0;
            try
            {
                string QueryFindID = "SELECT producerId FROM inventory.producer WHERE producerName = '" + producerName.ToLower() + "' LIMIT 1;";

                id = _db.FindID(QueryFindID, "ProducerId");
                if (id > -1)
                {
                    return id;
                }

                string QueryCreateProducer = "INSERT INTO inventory.producer(producername) VALUES ('" + producerName.ToLower() + "')";
                _db.Create(QueryCreateProducer);


                QueryFindID = "SELECT producerId FROM inventory.producer WHERE ProducerName = '" + producerName.ToLower() + "' LIMIT 1;";

                int returnvaluefromthedatabase = _db.FindID(QueryFindID, "ProducerId");
                if (returnvaluefromthedatabase > 0)
                {
                    return returnvaluefromthedatabase;
                }


            }
            catch
            {
                id = 0;
            }

            return id;
        }

        private int FindOrCreateSubCategory(string subcategoryName, int categoryId)
        {
            int id = 0;
            try
            {
                string QueryFindID = "SELECT subcategoryId FROM inventory.subcategory WHERE subcategoryName = '" + subcategoryName.ToLower() + "' LIMIT 1;";

                id = _db.FindID(QueryFindID, "SubCategoryId");
                if (id > -1)
                {
                    return id;
                }

                string QueryCreateSubCategory = "INSERT INTO inventory.subcategory(subcategoryname, category_id) VALUES ('" + subcategoryName.ToLower() + "'," + categoryId + ") ";
                _db.Create(QueryCreateSubCategory);


                QueryFindID = "SELECT subcategoryId FROM inventory.subcategory WHERE subcategoryName = '" + subcategoryName.ToLower() + "' LIMIT 1;";

                int returnvaluefromthedatabase = _db.FindID(QueryFindID, "SubCategoryId");
                if (returnvaluefromthedatabase > 0)
                {
                    return returnvaluefromthedatabase;
                }


            }
            catch
            {
                id = 0;
            }

            return id;
        }

        private int FindOrCreateModel(string modelName, int producerId)
        {
            int id = 0;
            try
            {
                string queryFindId = "SELECT modelId FROM inventory.model WHERE modelName = '" + modelName.ToLower() + "' LIMIT 1;";

                id = _db.FindID(queryFindId, "ModelId");
                if (id > -1)
                {
                    return id;
                }

                string queryCreateModel = "INSERT INTO inventory.model(modelname, producer_id) VALUES ('" + modelName.ToLower() + "'," + producerId + ") ";
                _db.Create(queryCreateModel);


                queryFindId = "SELECT modelId FROM inventory.model WHERE modelName = '" + modelName.ToLower() + "' LIMIT 1;";

                int returnvaluefromthedatabase = _db.FindID(queryFindId, "ModelId");
                if (returnvaluefromthedatabase > 0)
                {
                    return returnvaluefromthedatabase;
                }


            }
            catch
            {
                id = 0;
            }

            return id;
        }

        
    }
}
