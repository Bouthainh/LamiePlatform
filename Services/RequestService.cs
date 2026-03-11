using BadeePlatform.Data;
using BadeePlatform.Models;
using Microsoft.EntityFrameworkCore;

namespace BadeePlatform.Services
{
    public class RequestService : IRequestService
    {
        private readonly BadeedbContext _context;

        public RequestService(BadeedbContext context)
        {
            _context = context;
        }

        public async Task<List<Child>> SearchChildrenAsync(string searchTerm, string educatorId)
        {
            var educator = await _context.Educators
                .FirstOrDefaultAsync(e => e.EducatorId == educatorId);

            if (educator == null) return new List<Child>();

            return await _context.Children
                .Include(c => c.School)
                .Include(c => c.Grade)
                .Include(c => c.Class)
                .Where(c =>
                    c.SchoolId == educator.SchoolId &&
                    (c.ChildId.Contains(searchTerm) ||
                     c.ChildName != null && c.ChildName.Contains(searchTerm)))
                .ToListAsync();
        }

        public async Task<(bool Success, string Message)> SendRequestAsync(string educatorId, string childId)
        {
            var child = await _context.Children
                .Include(c => c.ParentChildren)
                .FirstOrDefaultAsync(c => c.ChildId == childId);

            if (child == null)
                return (false, "الطفل غير موجود");

            var parentChild = child.ParentChildren.FirstOrDefault();
            if (parentChild == null)
                return (false, "لا يوجد ولي أمر مرتبط بهذا الطفل");

            var existingRequest = await _context.Requests
                .FirstOrDefaultAsync(r =>
                    r.EducatorId == educatorId &&
                    r.ChildId == childId &&
                    r.RequestStatus == "Pending");

            if (existingRequest != null)
                return (false, "تم إرسال طلب مسبقاً لهذا الطفل وهو قيد الانتظار");

            var approvedRequest = await _context.Requests
                .FirstOrDefaultAsync(r =>
                    r.EducatorId == educatorId &&
                    r.ChildId == childId &&
                    r.RequestStatus == "Approved");

            if (approvedRequest != null)
                return (false, "تم قبول طلبك مسبقاً لهذا الطفل");

            var request = new Request
            {
                EducatorId = educatorId,
                ChildId = childId,
                ParentId = parentChild.ParentId,
                RequestStatus = "Pending",
                SentAt = DateTime.Now
            };

            _context.Requests.Add(request);
            await _context.SaveChangesAsync();

            return (true, "تم إرسال الطلب بنجاح");
        }

        public async Task<List<Request>> GetPendingRequestsByParentAsync(string parentId)
        {
            return await _context.Requests
                .Include(r => r.Educator)
                .Include(r => r.Child)
                .Where(r => r.ParentId == parentId && r.RequestStatus == "Pending")
                .OrderByDescending(r => r.SentAt)
                .ToListAsync();
        }

        public async Task<(bool Success, string Message)> RespondToRequestAsync(Guid requestId, string status, string parentId)
        {
            var request = await _context.Requests
                .FirstOrDefaultAsync(r => r.RequestId == requestId && r.ParentId == parentId);

            if (request == null)
                return (false, "الطلب غير موجود");

            request.RequestStatus = status;

            if (status == "Approved")
            {
                var alreadyExists = await _context.EducatorPermissions
                    .AnyAsync(p => p.EducatorId == request.EducatorId && p.ChildId == request.ChildId);

                if (!alreadyExists)
                {
                    var permission = new EducatorPermission
                    {
                        RequestId = Guid.NewGuid(),
                        EducatorId = request.EducatorId!,
                        ChildId = request.ChildId,
                        ParentId = request.ParentId,
                        CreatedAt = DateTime.Now
                    };
                    _context.EducatorPermissions.Add(permission);
                }
            }

            await _context.SaveChangesAsync();

            return status == "Approved"
                ? (true, "تم قبول الطلب وتمت إضافة الطالب لقائمة المعلم")
                : (true, "تم رفض الطلب");
        }

        public async Task<List<Request>> GetSentRequestsByEducatorAsync(string educatorId)
        {
            return await _context.Requests
                .Include(r => r.Child)
                .Include(r => r.Parent)
                .Where(r => r.EducatorId == educatorId)
                .OrderByDescending(r => r.SentAt)
                .ToListAsync();
        }

        public async Task<List<Child>> GetEducatorStudentsAsync(string educatorId)
        {
            var permissions = await _context.EducatorPermissions
                .Include(p => p.Child)
                    .ThenInclude(c => c.Grade)
                .Include(p => p.Child)
                    .ThenInclude(c => c.Class)
                .Include(p => p.Child)
                    .ThenInclude(c => c.School)
                .Where(p => p.EducatorId == educatorId)
                .ToListAsync();

            return permissions
                .Where(p => p.Child != null)
                .Select(p => p.Child!)
                .ToList();
        }
    }
}