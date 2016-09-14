using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Constants
{
    /// <summary>
    /// Permission levels with integer value representations.
    /// </summary>

    public enum PermissionLevelsEnum
    {
        /// <summary>
        /// Root = 100
        /// </summary>

        Root = 100,

        /// <summary>
        /// 2nd highest permission level for fixed system attributes (e.g. IsPublished of a link item).
        /// </summary>

        RootMinus = 99,

        /// <summary>
        /// OrgAdmin = 90
        /// </summary>

        OrgAdmin = 90,

        /// <summary>
        /// OrgAdminMinus = 89
        /// </summary>

        OrgAdminMinus = 89,
        
        OrgAdminMinus2 = 88,
        
        OrgAdminMinus3 = 87,
        
        OrgAdminMinus4 = 86,
        
        OrgAdminMinus5 = 85,
        
        OrgAdminMinus6 = 84,
        
        OrgAdminMinus7 = 83,

        /// <summary>
        /// SubOrgAdmin = 80
        /// </summary>

        SubOrgAdmin = 80,

        /// <summary>
        /// SubOrgAdminMinus = 79. 
        /// This can be used to create DENY rules which override rules from 'above' (further back the nav path).
        /// <example>
        /// /Root has the following rule - CanView = Base : ALLOW : IsAnonymous
        /// /Root/MyPage must have the following rule in order to override Root-rule: CanView = SubOrgAdminMinus : DENY : IsAnonymous() 
        /// This rule doesn't override the local rules with permission level SubOrgAdmin.
        /// If there were a rule CanView = SubOrgAdmin : DENY : IsAnonymous(), all rules 
        /// with this level would be overridden because DENY is stronger than ALLOW.
        /// </example>
        /// </summary>

        SubOrgAdminMinus = 79,
        
        SubOrgAdminMinus2 = 78,
        
        SubOrgAdminMinus3 = 77,
        
        SubOrgAdminMinus4 = 76,
        
        SubOrgAdminMinus5 = 75,
        
        SubOrgAdminMinus6 = 74,
        
        SubOrgAdminMinus7 = 73,

        /// <summary>
        /// Course = 60
        /// </summary>

        Course = 60,

        /// <summary>
        /// CourseMinus = 59
        /// </summary>

        CourseMinus = 59,

        /// <summary>
        /// SubCourse = 55,
        /// </summary>

        SubCourse = 55,

        /// <summary>
        /// SubCourseMinus = 54
        /// </summary>

        SubCourseMinus = 54,

        /// <summary>
        /// Instructor = 50
        /// </summary>

        Instructor = 50,

        /// <summary>
        /// InstructorMinus = 49
        /// </summary>

        InstructorMinus = 49,

        /// <summary>
        /// Student = 20
        /// </summary>

        Student = 20,

        /// <summary>
        /// StudentMinus = 19
        /// </summary>

        StudentMinus = 19,

        /// <summary>
        /// BaseStudent = 17. Base permission level for <c>Student</c>.
        /// </summary>

        BaseStudent = 17,

        /// <summary>
        /// BaseInstructor = 10. Base permission level for <c>Instructor</c>.
        /// </summary>

        BaseInstructor = 10,

        /// <summary>
        /// BaseSubCourse = 9. Base permission level for <c>SubCourse</c>.
        /// </summary>

        BaseSubCourse = 9,

        /// <summary>
        /// BaseCourse = 8. Base permission level for <c>Course</c>.
        /// </summary>

        BaseCourse = 8,

        /// <summary>
        /// BaseSubOrgAdmin = 17. Base permission level for <c>SubOrgAdmin</c>.
        /// </summary>

        BaseSubOrgAdmin = 7,

        /// <summary>
        /// BaseOrgAdmin= 5. Base permission level for <c>OrgAdmin</c>.
        /// </summary>

        BaseOrgAdmin = 5,

        /// <summary>
        /// Base = 0. The lowest permission level.
        /// </summary>

        Base = 0,

        /// <summary>
        /// Bottom = -1
        /// This is not allowed to be set as permission level on an attribute.
        /// </summary>

        Bottom = -1
    }

    /// <summary>
    /// Access states like NONE, ALLOW and DENY
    /// </summary>

    public enum AccessStateEnum
    {
        /// <summary>
        /// NONE - access state unknown.
        /// This state is not allowed to be set as access on an attribute.
        /// </summary>

        NONE = 1,

        /// <summary>
        /// ALLOW - allow access.
        /// </summary>

        ALLOW = 2,

        /// <summary>
        /// DENY - deny access.
        /// </summary>

        DENY = 4
    }
}
