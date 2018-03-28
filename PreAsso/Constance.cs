using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PreAsso
{
    public class Constance
    {
        public static string Header70 = @"@RELATION	medical
                                        @ATTRIBUTE	Tuổi	{thanh_niên,trung_niên,lão_niên}
                                        @ATTRIBUTE	Bệnh_viện	{Bệnh_viện_đa_khoa_tỉnh_Sơn_La,Bệnh_viện_đa_khoa_huyện_Mai_Sơn,Bệnh_viện_đa_khoa_huyện_Yên_Châu,Bệnh_viện_đa_khoa_Mộc_Châu,Bệnh_viện_đa_khoa_huyện_Mường_La,Bệnh_viện_đa_khoa_huyện_Thuận_Châu}
                                        @ATTRIBUTE	Khoa	{Khoa_Nội_Tổng_Hợp,HSTC,Hồi_Sức_Cấp_Cứu,Khoa_Tim_Mạch,Đơn_nguyên_chăm_sóc_đặc_biệt,Cấp_Cứu,Khoa_Nội_-_Hồi_Sức_Cấp_Cứu,PK_Mãn_Tính,Khám_Bệnh,PK_Tim_Mạch,Câp_Cứu,Khoa_Ngoại_Tổng_Hợp}
                                        @ATTRIBUTE	Giới_tính	{Nữ,Nam}
                                        @ATTRIBUTE	Nghề_nghiệp	{Nông_Dân,Nghỉ_Hưu,Cán_Bộ,Nhân_Dân,Học_Sinh,Công_Nhân,Giáo_Viên,Bác_Sỹ,Sinh_Viên,Công_An}
                                        @ATTRIBUTE	Dân_tộc	{Kinh,Thái,Mường,H.Mông,Lào,Sinh_Mun,Dáy,Hoa,Tày,Kháng,Lào}
                                        @ATTRIBUTE	Thành_phố	{Sơn_La,Hòa_Bình}
                                        @ATTRIBUTE	Tỉnh	{Sơn_La,NAM_ĐỊNH,HÀ_NỘI,Thanh_Hóa,Hà_Nam,Ninh_Bình,Nghệ_An}
                                        @ATTRIBUTE	5._Tuổi_xuất_hiện_cao_huyết_áp	{Trên_40,Từ_15_-_40}
                                        @ATTRIBUTE	Mạch_toàn_thân_(lần/phút)	{thấp,bình_thường,cao}
                                        @ATTRIBUTE	Nhiệt_độ_(độ_C)	{thấp,bình_thường,cao}
                                        @ATTRIBUTE	Huyết_áp_ngưỡng_thấp_(mmHg)	{bình_thường,cao}
                                        @ATTRIBUTE	Huyết_áp_ngưỡng_cao_(mmHg)	{bình_thường,cao}
                                        @ATTRIBUTE	Nhịp_thở_(lần/phút)	{thấp,bình_thường,cao}
                                        @ATTRIBUTE	Glucose_máu	{thấp,bình_thường,cao}
                                        @ATTRIBUTE	Ure	{thấp,bình_thường,cao}
                                        @ATTRIBUTE	Creatinin	{thấp,bình_thường,cao}
                                        @ATTRIBUTE	7._Cholesteol_bao_nhiêu	{Cao,Bình_thường,Thấp}
                                        @ATTRIBUTE	8._Triglycerid_bao_nhiêu_:	{Cao,Bình_thường,Thấp}
                                        @ATTRIBUTE	Phân_loại	{THA_độ_III_(_110_=<_HA_=<_180),THA_độ_I_(_90_-_99_<_HA_<_140_-_159),THA_độ_II_(_100_-_109_<_HA_<_160_-_179),HA_bình_thường_cao_(_85_-_89_<_HA_<_130_-_139),HA_bình_thường_(_85_<_HA_<_130_),HA_tối_ưu_(_80_<_HA_<_120_),THA_tâm_thu_đơn_độc_(_90_<_HA_=<_140)}
                                        @Data";

        public static string Header50 = @"@RELATION	medical
                                        @ATTRIBUTE	Tuổi	{thanh_niên,trung_niên,lão_niên}
                                        @ATTRIBUTE	Bệnh_viện	{Bệnh_viện_đa_khoa_tỉnh_Sơn_La,Bệnh_viện_đa_khoa_huyện_Mai_Sơn,Bệnh_viện_đa_khoa_huyện_Yên_Châu,Bệnh_viện_đa_khoa_Mộc_Châu,Bệnh_viện_đa_khoa_huyện_Mường_La,Bệnh_viện_đa_khoa_huyện_Thuận_Châu}
                                        @ATTRIBUTE	Khoa	{Khoa_Nội_Tổng_Hợp,HSTC,Hồi_Sức_Cấp_Cứu,Khoa_Tim_Mạch,Đơn_nguyên_chăm_sóc_đặc_biệt,Cấp_Cứu,Khoa_Nội_-_Hồi_Sức_Cấp_Cứu,PK_Mãn_Tính,Khám_Bệnh,PK_Tim_Mạch,Câp_Cứu,Khoa_Ngoại_Tổng_Hợp}
                                        @ATTRIBUTE	Giới_tính	{Nữ,Nam}
                                        @ATTRIBUTE	Nghề_nghiệp	{Nông_Dân,Nghỉ_Hưu,Cán_Bộ,Nhân_Dân,Học_Sinh,Công_Nhân,Giáo_Viên,Bác_Sỹ,Sinh_Viên,Công_An}
                                        @ATTRIBUTE	Dân_tộc	{Kinh,Thái,Mường,H.Mông,Lào,Sinh_Mun,Dáy,Hoa,Tày,Kháng,Lào}
                                        @ATTRIBUTE	Huyện	{Mai_Sơn,Yên_Châu,Mộc_Châu,Sông_Mã,Mường_La,Thuận_Châu,Quỳnh_Nhai,Bắc_Yên,Phù_Yên,Vân_Hồ,Sốp_Cộp}
                                        @ATTRIBUTE	Thành_phố	{Sơn_La,Hòa_Bình}
                                        @ATTRIBUTE	Tỉnh	{Sơn_La,NAM_ĐỊNH,HÀ_NỘI,Thanh_Hóa,Hà_Nam,Ninh_Bình,Nghệ_An}
                                        @ATTRIBUTE	1._Bản_thân_có_mắc_bệnh_đái_tháo_đường_không_rối_loạn_lipit_máu_bệnh_mạch_vành_bệnh_thận_có_hút_thuốc_lá_không	{Không,Có}
                                        @ATTRIBUTE	5._Tuổi_xuất_hiện_cao_huyết_áp	{Trên_40,Từ_15_-_40}
                                        @ATTRIBUTE	Mạch_toàn_thân_(lần/phút)	{thấp,bình_thường,cao}
                                        @ATTRIBUTE	Nhiệt_độ_(độ_C)	{thấp,bình_thường,cao}
                                        @ATTRIBUTE	Huyết_áp_ngưỡng_thấp_(mmHg)	{bình_thường,cao}
                                        @ATTRIBUTE	Huyết_áp_ngưỡng_cao_(mmHg)	{bình_thường,cao}
                                        @ATTRIBUTE	Nhịp_thở_(lần/phút)	{thấp,bình_thường,cao}
                                        @ATTRIBUTE	Glucose_máu	{thấp,bình_thường,cao}
                                        @ATTRIBUTE	Ure	{thấp,bình_thường,cao}
                                        @ATTRIBUTE	Acid_uric	{bình_thường,cao}
                                        @ATTRIBUTE	Creatinin	{thấp,bình_thường,cao}
                                        @ATTRIBUTE	7._Cholesteol_bao_nhiêu	{Cao,Bình_thường,Thấp}
                                        @ATTRIBUTE	8._Triglycerid_bao_nhiêu_:	{Cao,Bình_thường,Thấp}
                                        @ATTRIBUTE	9._HDL-C_bao_nhiêu	{Bình_thường,Không_bình_thường}
                                        @ATTRIBUTE	10._LDL-C_bao_nhiêu	{Bình_thường,Cao_hơn_bình_thường,Thấp_hơn_bình_thường}
                                        @ATTRIBUTE	Phân_loại	{THA_độ_III_(_110_=<_HA_=<_180),THA_độ_I_(_90_-_99_<_HA_<_140_-_159),THA_độ_II_(_100_-_109_<_HA_<_160_-_179),HA_bình_thường_cao_(_85_-_89_<_HA_<_130_-_139),HA_bình_thường_(_85_<_HA_<_130_),HA_tối_ưu_(_80_<_HA_<_120_),THA_tâm_thu_đơn_độc_(_90_<_HA_=<_140)}
                                        @Data";
    }
}
