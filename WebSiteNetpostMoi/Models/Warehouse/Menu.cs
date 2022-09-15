using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebSite.Language;

namespace WebSite.Models.Warehouse
{
    public class Menu
    {
        public static List<CSS_DANH_SACH_MENUDTO> GetDepartments()
        {
            return new List<CSS_DANH_SACH_MENUDTO>() {
                //CreateDepartment(1, 0,  "Warehouse", "mnuWarehouse", "","WAREHOUSE"),
                CreateDepartment(2, 0,  Booking.WH_quanLyDanhSachKhachHang, "mnuQuanLyDanhSachKhohang", "/Warehouse/QuanLyDanhSachKhohang","WAREHOUSE"),
                CreateDepartment(3, 0,  Booking.WH_quanLyDanhMucNhomSP, "mnuQuanLyDanhMucNhomSanPham", "/Warehouse/QuanLyDanhMucNhomSP","WAREHOUSE"),
                CreateDepartment(4, 0,  Booking.WH_quanLyDanhMucSP, "mnuQuanLyDanhMucSanPham", "/Warehouse/QuanLyDanhMucSanPham","OTSUWAREHOUSEKA"),
                CreateDepartment(5, 0,  Booking.WH_quanLyNhapKho, "mnuQuanLyNhapKho", "/Warehouse/QuanLyNhapKho","WAREHOUSE"),
                CreateDepartment(6, 0,  Booking.WH_quanLyXuatKho, "mnuQuanLyXuatKho", "/Warehouse/QuanLyXuatKho","WAREHOUSE"),
                CreateDepartment(7, 0,  Booking.WH_baoCaoXuatNhapTon, "mnuBaoCaoXuatNhapTon", "/Warehouse/BaoCaoXuatNhapTon","WAREHOUSE")
            };
        }
        public static List<TreeNode> GetAllCategoriesForTree(string TimKiem = "")
        {
            List<CSS_DANH_SACH_MENUDTO> categories = new List<CSS_DANH_SACH_MENUDTO>();
            List<CSS_DANH_SACH_MENUDTO> lst = new List<CSS_DANH_SACH_MENUDTO>();
            if (!string.IsNullOrWhiteSpace(TimKiem))
            {
                categories = GetDepartments().Where(x => x.Name.ToLower().Contains(TimKiem.ToLower())).ToList();
                foreach (var item in categories)
                {
                    var nodes = categories.FirstOrDefault(x => x.ID == item.ParentID);
                    if (nodes == null)
                    {
                        if (lst.FirstOrDefault(x => x.ID == item.ParentID) == null)
                        {
                            lst.Add(GetDepartments().FirstOrDefault(x => x.ID == item.ParentID));
                        }
                    }
                }
                lst.AddRange(categories);
            }
            else
            {
                categories = GetDepartments();
                lst = categories;
            }
            List<TreeNode> headerTree = FillRecursive(lst, 0);

            return headerTree;
        }
        private static List<TreeNode> FillRecursive(List<CSS_DANH_SACH_MENUDTO> flatObjects, int? parentId = null)
        {
            var user = HttpContext.Current.Session["User"] as CSS_CRM_NGUOI_DUNGDTO;
            if (user != null)
            {
                return flatObjects.Where(x => x.ParentID.Equals(parentId)).Select(item => new TreeNode
                {
                    Code = item.Code,
                    ID = item.ID,
                    Name = item.Name,
                    Url = item.Url,
                    Children = FillRecursive(flatObjects, item.ID)
                }).ToList();
            }
            return null;

        }

        static CSS_DANH_SACH_MENUDTO CreateDepartment(int id, int parentID, string text, string name, string Url, string Role)
        {
            return new CSS_DANH_SACH_MENUDTO
            {
                ID = id,
                ParentID = parentID,
                Code = name,
                Name = text,
                Url = Url,
                Role = Role
            };
        }


        public static List<TreeNode> GetAllCategoriesForTreeTest(string TimKiem = "")
        {
            var user = HttpContext.Current.Session["User"] as CSS_CRM_NGUOI_DUNGDTO;
            List<CSS_DANH_SACH_MENUDTO> categories = new List<CSS_DANH_SACH_MENUDTO>();
            List<CSS_DANH_SACH_MENUDTO> lst = new List<CSS_DANH_SACH_MENUDTO>();

            lst = user.lstMenu;
            List<TreeNode> headerTree = FillRecursiveTest(lst, 0);

            return headerTree;
        }
        private static List<TreeNode> FillRecursiveTest(List<CSS_DANH_SACH_MENUDTO> flatObjects, int? parentId = null)
        {

            return flatObjects.Where(x => x.ParentID.Equals(parentId)).Select(item => new TreeNode
            {
                Code = item.Code,
                ID = item.ID,
                Name = item.Name,
                Url = item.Url,
                Children = FillRecursiveTest(flatObjects, item.ID)
            }).ToList();


        }

    }
    public class TreeNode
    {
        public int ID { get; set; }
        public int ParentID { get; set; }
        public String Code { get; set; }
        public String Name { get; set; }
        public String Url { get; set; }

        public TreeNode ParentCategory { get; set; }
        public List<TreeNode> Children { get; set; }
    }
}