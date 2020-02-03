using BaTecInventory.Models;
using System;
using System.Collections.Generic;

using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.ModelBinding;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.Expressions;
using Microsoft.Ajax.Utilities;
using PagedList;
using PagedList.Mvc;
using WebGrease.Css.Extensions;
using ClosedXML.Excel;



namespace BaTecInventory.Controllers
{

    public class InventoryController : Controller
    {
        DB.DBConnect _db = new DB.DBConnect();

        private List<List<string>> tempData;
        private List<Product> Product;
        private List<Product> returnList;



        public InventoryController()
        {

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
            returnList = new List<Product>();



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
                    //Category = _db.Select("" +
                    //"SELECT cat.categoryname " +
                    //"FROM inventory.category AS cat" +
                    //"INNER JOIN inventory.SubCategory AS sub " +
                    //"ON sub.Category_Id = cat.CategoryId" +
                    //"WHERE sub.SubCategoryId = " + x[10]),
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

        // GET: Inventory
        public ActionResult Index(int? page, string sortBy, string searchPar, string search)
        {
            returnList = new List<Product>();


            ViewBag.SortNameParameter = sortBy == "UserName" ? "UserName desc" : "UserName";
            ViewBag.SortModelParameter = sortBy == "Model" ? "Model desc" : "Model";
            ViewBag.SortSupplierParameter = sortBy == "Supplier" ? "Supplier desc" : "Supplier";
            ViewBag.SortProducerParameter = sortBy == "Producer" ? "Producer desc" : "Producer";
            ViewBag.SortCategoryParameter = sortBy == "Category" ? "Category desc" : "Category";
            ViewBag.SortSubCategoryParameter = sortBy == "SubCategory" ? "SubCategory desc" : "SubCategory";
            ViewBag.SortPurchaseDateParameter = string.IsNullOrEmpty(sortBy) ? /*sortBy == "PurchaseDate" ?*/ "PurchaseDate desc" : " ";


            //string searchParameter = "Searching";

            //if (searchBy == "Supplier")
            //{
            //    prod.Select(x => x.Supplier == search || search == null);
            //}

            //else
            //{
            //    prod.Select(x => x.UserName == search || search == null);
            //}

            //switch (sortBy)
            //{
            //    case "UserName desc":
            //        prod.OrderByDescending(x => x.UserName);
            //        break;


            //    case "Supplier desc":
            //        prod.OrderByDescending(x => x.Supplier);
            //        break;

            //    case "Supplier":
            //        prod.OrderBy(x => x.Supplier);
            //        break;
            //    default:
            //        prod.OrderBy(x => x.UserName);
            //        break;

            //}

            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                try
                {
                    switch (sortBy)
                    {
                        case
                        "UserName desc":
                            foreach (Product temp in Product.OrderByDescending(x => x.UserName))
                            {
                                returnList.Add(temp);

                            }
                            break;
                        case
                            "Supplier desc":
                            foreach (Product temp in Product.OrderByDescending(x => x.Supplier))
                            {
                                returnList.Add(temp);
                            }
                            break;
                        case
                            "Model desc":
                            foreach (Product temp in Product.OrderByDescending(x => x.Model))
                            {
                                returnList.Add(temp);
                            }
                            break;
                        case
                        "Producer desc":
                            foreach (Product temp in Product.OrderByDescending(x => x.Producer))
                            {
                                returnList.Add(temp);
                            }
                            break;
                        case
                            "Category desc":
                            foreach (Product temp in Product.OrderByDescending(x => x.Category))
                            {
                                returnList.Add(temp);
                            }
                            break;
                        case
                            "SubCategory desc":
                            foreach (Product temp in Product.OrderByDescending(x => x.SubCategory))
                            {
                                returnList.Add(temp);
                            }
                            break;
                        default:
                            foreach (Product temp in Product.OrderByDescending(x => x.PurchaseDate))
                            {
                                returnList.Add(temp);
                            }
                            break;


                    }

                    return View(returnList.ToPagedList(page ?? 1, 3));
                }
                catch (Exception e)
                {
                    string error = e.Message;
                }
            }


            ViewData["Product"] = tempData;

            if (tempData == null)
            {
                return HttpNotFound();
            }



            return View(Product.ToPagedList(page ?? 1, 3));
        }

        //public JsonResult ProductSerialExist(string serialNumber)
        //{
        //    return Json(!Product.Any(e => e.SerialNumber == serialNumber), JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult ProductSerialExits(string SerialNumber)
        //{
        //    bool any = false;
        //    foreach (var e in Product)
        //    {
        //        if (e.SerialNumber == SerialNumber)
        //        {
        //            any = true;
        //            break;
        //        }
        //    }
        //    return Json(!any, JsonRequestBehavior.AllowGet);
        //}

        public void ExportToExcel()
        {
            Helper.ExcelManager.ExportExcel();
            var gv = new GridView
            {
                DataSource = Product

            };
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=inventory_list.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = " ";
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
            //gv.AllowCustomPaging = false;
            //gv.AllowPaging = true;
            gv.RenderControl(objHtmlTextWriter);
            Response.Output.Write(objStringWriter.ToString());
            Response.Flush();
            Response.End();

        }

        //[HttpPost]
        public void SearchExportToExcel()
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
            gv.RenderControl(objHtmlTextWriter);
            Response.Output.Write(objStringWriter.ToString());
            Response.Flush();
            Response.End();

        }

        
        public ActionResult SearchProduct(string searchPar, int? page)
        {
            returnList = new List<Product>();

            int searchCase = Convert.ToInt32(Request["searchBy"]);
            string searchParameter = "Searching";

            if (!string.IsNullOrWhiteSpace(searchPar))
            {

                try
                {

                    switch (searchCase)
                    {
                        case 0:
                            foreach (Product temp in Product)
                            {
                                if (temp.ProductId.ToString() == searchPar)
                                    returnList.Add(temp);
                            }
                            break;

                        case 1:
                            foreach (Product temp in Product)
                            {
                                if (temp.UserName == searchPar)
                                    returnList.Add(temp);
                            }
                            break;
                    }
                }
                catch (Exception e)
                {
                    string error = e.Message;
                }


            }

            ViewBag.SearchParamater = searchParameter;
            Session["returnList"] = returnList.ToList();
            return View(returnList.ToPagedList(page ?? 1, 3));

        }


        // GET: Product/Details/5
        public ActionResult Details(int? id)
        {
            var pdt = Product.Find(m => m.ProductId == id);
            return View(pdt);

        }



        public ActionResult GetSearchSupplier()
        {
            // Get all suppliers from the database and show those

            string query = "" +
                "SELECT suppliername " +
                "FROM inventory.supplier";

            List<ItemName> theListThatareReturningToTheView = new List<ItemName>();

            List<string> values = _db.AllList(query);

            foreach (string val in values)
            {
                ItemName temperoryInstansOfTheValueFromTheForeachLoop = new ItemName
                {
                    GetName = val
                };
                theListThatareReturningToTheView.Add(temperoryInstansOfTheValueFromTheForeachLoop);
            }

            // Sort all the items and make a new list to show in the view

            return new JsonResult { Data = theListThatareReturningToTheView, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult GetSearchCategory()
        {


            string query = "SELECT categoryname FROM inventory.category";

            List<ItemName> theListThatareReturningToTheView = new List<ItemName>();

            List<string> values = _db.AllList(query);

            foreach (string val in values)
            {
                ItemName temperoryInstansOfTheValueFromTheForeachLoop = new ItemName
                {
                    GetName = val
                };
                theListThatareReturningToTheView.Add(temperoryInstansOfTheValueFromTheForeachLoop);
            }



            return new JsonResult { Data = theListThatareReturningToTheView, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult GetSearchSubCategory()
        {


            string query = "SELECT subcategoryname FROM inventory.subcategory";

            List<ItemName> theListThatareReturningToTheView = new List<ItemName>();

            List<string> values = _db.AllList(query);

            foreach (string val in values)
            {
                ItemName temperoryInstansOfTheValueFromTheForeachLoop = new ItemName
                {
                    GetName = val
                };
                theListThatareReturningToTheView.Add(temperoryInstansOfTheValueFromTheForeachLoop);
            }



            return new JsonResult { Data = theListThatareReturningToTheView, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult GetSearchProducer()
        {


            string query = "SELECT producername FROM inventory.producer";

            List<ItemName> theListThatareReturningToTheView = new List<ItemName>();

            List<string> values = _db.AllList(query);

            foreach (string val in values)
            {
                ItemName temperoryInstansOfTheValueFromTheForeachLoop = new ItemName
                {
                    GetName = val
                };
                theListThatareReturningToTheView.Add(temperoryInstansOfTheValueFromTheForeachLoop);
            }



            return new JsonResult { Data = theListThatareReturningToTheView, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult GetSearchModel()
        {

            string query = "SELECT modelname FROM inventory.model";

            List<ItemName> theListThatareReturningToTheView = new List<ItemName>();

            List<string> values = _db.AllList(query);

            foreach (string val in values)
            {
                ItemName temperoryInstansOfTheValueFromTheForeachLoop = new ItemName
                {
                    GetName = val
                };
                theListThatareReturningToTheView.Add(temperoryInstansOfTheValueFromTheForeachLoop);
            }

            return new JsonResult { Data = theListThatareReturningToTheView, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        
        public ActionResult CreateProduct()
        {
            ViewBag.userlist = _db.Users().Select(m => m.UserName);
            return View();
        }


        // POST: Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateProduct(Product product)
        {
            //if (SerialExist(product.SerialNumber))
            //{
            //    var query = "SELECT count(ProductId) FROM inventory.product WHERE SerialNumber = ('" +
            //               product.SerialNumber + "') LIMIT 1 ";
            //    _db.DataValidation(query);
            //    ModelState.AddModelError("Serial", " Product Serial already exists.");
            //}

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
            int userid = FindOrCreateUser(product.UserName);
            if (userid == 0)
            {
                return View();
            }
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

            if (ModelState.IsValid)
            {
                var query =
                    "INSERT INTO  inventory.Product (serialnumber, receiptnumber, purchasedate, destructiondate, comments, history,User_Id, Model_Id, Supplier_Id, SubCategory_Id) VALUES ('" +
                    product.SerialNumber + "', '" + product.ReceiptNumber + "','" +
                    product.PurchaseDate.ToString("yyyy-MM-dd") + "','" +
                    product.DestructionDate.ToString("yyyy-MM-dd") + "', '" + product.Comments + "', '" +
                    product.History + "', '" + userid + "','" + modelid + "','" + supplierid + "','" + subcategoryid +
                    "' ) ";


                _db.Create(query);

                return RedirectToAction("Index");


            }



            return View();
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
                 *          - Execute the query that, created thro the database
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

        private int FindOrCreateUser(string userName)
        {
            int id = 0;
            try
            {
                string QueryFindID = "SELECT userId FROM inventory.user WHERE userName = '" + userName.ToLower() + "' LIMIT 1;";

                id = _db.FindID(QueryFindID, "UserId");
                if (id > -1)
                {
                    return id;
                }

                string QueryCreateUser = "INSERT INTO inventory.user(username) VALUES ('" + userName.ToLower() + "')";
                _db.Create(QueryCreateUser);


                QueryFindID = "SELECT userId FROM inventory.user WHERE UserName = '" + userName.ToLower() + "' LIMIT 1;";

                int returnvaluefromthedatabase = _db.FindID(QueryFindID, "UserId");
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


        // GET: Product/Edit/5
        public ActionResult Edit(int? id)
        {
            ViewBag.userlist = _db.Users().Select(m => m.UserName);
            var pdt = Product.Find(m => m.ProductId == id);

            return View(pdt);

        }


        // POST: Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Product product)

        {
            //if (SerialExist(product.SerialNumber))
            //{
            //    var query = "SELECT count(ProductId) FROM inventory.product WHERE SerialNumber = ('" +
            //               product.SerialNumber + "') LIMIT 1 ";
            //    _db.DataValidation(query);
            //    ModelState.AddModelError(string.Empty, " Product Serial already exists.");
            //}

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
            int userid = FindOrCreateUser(product.UserName);
            if (userid == 0)
            {
                return View();
            }
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
                var query = "UPDATE inventory.Product Set SerialNumber = ('" + product.SerialNumber + "'),ReceiptNumber=('" + product.ReceiptNumber + "'),PurchaseDate=('" + product.PurchaseDate.ToString("yyyy-MM-dd") + "'),DestructionDate=('" + product.DestructionDate.ToString("yyyy-MM-dd") + "'),Comments=('" + product.Comments + "'),History=('" + product.History + "')," +
                            " user_id = ('" + userid + "'), model_id = ('" + modelid + "'),supplier_id = ('" + supplierid + "'), subcategory_id = ('" + subcategoryid + "') Where ProductId = ('" + pdt.ProductId + "')";
                _db.Edit(query);

                return RedirectToAction("Index");
            }


            return View(product);
        }

        //GET: Product/Delete/5
        public ActionResult Delete(int? id)
        {
            var prod = Product.Find(m => m.ProductId == id);
            return View(prod);
        }

        //POST: Product/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, Product prod)
        {
            prod = Product.Find(m => m.ProductId == id);
            var query = "DELETE  FROM inventory.product Where productid = ('" + prod.ProductId + "')";
            _db.Delete(query);
            return RedirectToAction("Index");
        }


        private bool SerialExist(string serial)
        {
            return Product.Any(e => e.SerialNumber == serial);
        }

    }


}