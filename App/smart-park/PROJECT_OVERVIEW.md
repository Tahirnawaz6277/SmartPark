# SmartPark Project - Complete Overview

## 📋 Project Summary

**Project Name**: SmartPark (smart-part)  
**Framework**: Angular 20.2.0  
**Language**: TypeScript 5.9.2  
**Styling**: SCSS + Bootstrap 5.3.8 + Angular Material 20.2.9  
**Backend API**: https://localhost:7188/api/  
**Package Manager**: npm  

---

## 🏗️ Project Architecture

### **Application Type**
- **Role-Based Multi-User System** with separate dashboards for:
  - **Admin**: Full system management
  - **Driver**: Personal bookings and billings

### **Key Features Implemented**
✅ JWT Authentication with auto-expiry handling  
✅ Role-based routing with guards  
✅ HTTP Interceptors (Auth + Error handling)  
✅ Profile management with image upload  
✅ CRUD operations for Users  
✅ Responsive UI with Bootstrap & Material Design  
✅ Lazy-loaded modules for performance  

---

## 📁 Folder Structure

```
smart-park/
├── src/
│   ├── app/
│   │   ├── api/                    # API Services Layer
│   │   │   ├── billing/            # Billing API service
│   │   │   ├── booking/            # Booking API service
│   │   │   ├── location/           # Location API service
│   │   │   └── user/               # User API service
│   │   │
│   │   ├── core/                   # Core Application Logic
│   │   │   ├── guards/             # Route guards (role-guard)
│   │   │   ├── interceptors/       # HTTP interceptors
│   │   │   │   ├── auth.interceptor.ts
│   │   │   │   └── error.interceptor.ts
│   │   │   ├── models/             # TypeScript interfaces/models
│   │   │   └── services/           # Core services
│   │   │       ├── error-handler.service.ts
│   │   │       └── profile.service.ts
│   │   │
│   │   ├── pages/                  # Feature Modules
│   │   │   ├── admin/              # Admin Module
│   │   │   │   ├── admin-dashboard/
│   │   │   │   ├── admin-layout/
│   │   │   │   ├── billings/       # Billing management
│   │   │   │   ├── bookings/       # Booking management
│   │   │   │   ├── locations/      # Location management
│   │   │   │   └── users/          # User management (CRUD complete)
│   │   │   │
│   │   │   ├── driver/             # Driver Module
│   │   │   │   ├── driver-dashboard/
│   │   │   │   ├── driver-layout/
│   │   │   │   ├── my-billings/
│   │   │   │   └── my-bookings/
│   │   │   │
│   │   │   ├── auth/               # Authentication
│   │   │   │   ├── login/
│   │   │   │   └── signup/
│   │   │   │
│   │   │   └── dashboard/          # Legacy dashboard (backward compatibility)
│   │   │
│   │   ├── shared/                 # Shared Components
│   │   │   ├── footer/
│   │   │   ├── header/
│   │   │   ├── navbar/             # Profile modal included
│   │   │   ├── sidebar/
│   │   │   ├── master-layout/
│   │   │   └── page-not-found/
│   │   │
│   │   ├── app.config.ts           # App configuration (interceptors registered)
│   │   ├── app.routes.ts           # Main routing configuration
│   │   └── app.ts                  # Root component
│   │
│   ├── environments/               # Environment configurations
│   │   ├── environment.ts          # Development
│   │   └── environment.prod.ts     # Production
│   │
│   ├── assets/                     # Static assets
│   ├── styles.scss                 # Global styles
│   └── main.ts                     # Application entry point
│
├── node_modules/                   # ✅ Dependencies installed
├── package.json                    # Project dependencies
├── angular.json                    # Angular CLI configuration
├── tsconfig.json                   # TypeScript configuration
└── README.md                       # Project documentation
```

---

## 🔑 Key Technologies & Dependencies

### **Core Dependencies**
```json
{
  "@angular/core": "^20.2.0",
  "@angular/material": "^20.2.9",
  "@angular/router": "^20.2.0",
  "bootstrap": "^5.3.8",
  "jwt-decode": "^4.0.0",
  "rxjs": "~7.8.0"
}
```

### **Dev Dependencies**
```json
{
  "@angular/cli": "^20.2.2",
  "typescript": "~5.9.2",
  "karma": "~6.4.0",
  "jasmine-core": "~5.9.0"
}
```

---

## 🛣️ Routing Structure

### **Public Routes**
- `/` → Redirects to `/login`
- `/login` → Login page
- `/signup` → Signup page
- `/**` → 404 Page Not Found

### **Admin Routes** (Protected: Role = 'Admin')
- `/admin/dashboard` → Admin dashboard
- `/admin/users` → User management (View/Add/Edit/Delete) ✅
- `/admin/bookings` → Booking management
- `/admin/locations` → Location management
- `/admin/billings` → Billing management

### **Driver Routes** (Protected: Role = 'Driver')
- `/driver/dashboard` → Driver dashboard
- `/driver/my-bookings` → Personal bookings
- `/driver/my-billings` → Personal billings

---

## 🔐 Security Features

### **1. Authentication System**
- JWT token-based authentication
- Token stored in `localStorage` with key: `auth_token`
- Automatic token expiry detection
- Auto-redirect to login on 401 responses

### **2. HTTP Interceptors**

#### **Auth Interceptor** (`auth.interceptor.ts`)
- Adds `Authorization: Bearer {token}` to all API requests
- Catches 401 errors
- Clears localStorage and redirects to login

#### **Error Interceptor** (`error.interceptor.ts`)
- Logs all HTTP requests with timing
- Provides user-friendly error messages
- Handles client-side and server-side errors
- Console logging for debugging

### **3. Route Guards**
- `roleGuard` - Protects routes based on user role
- Validates JWT token and role before allowing access

---

## 🎨 UI/UX Features

### **Design System**
- **Bootstrap 5.3.8** for responsive grid and utilities
- **Angular Material 20.2.9** for components (modals, forms)
- **SCSS** for custom styling
- **Responsive Design** - Mobile-first approach

### **Components**
- **Navbar**: Profile image display, dropdown menu, profile modal
- **Sidebar**: Role-based navigation menu
- **Footer**: Application footer
- **Master Layout**: Wrapper for authenticated pages

### **Profile Management**
- Profile image display in navbar (32x32 circle)
- Profile modal with user details
- Image upload with preview
- FormData-based file upload
- Real-time image refresh

---

## 📡 API Integration

### **Base URL**: `https://localhost:7188/api/`

### **Implemented APIs**

#### **User Management**
- `GET /api/User/get-all-users` - Fetch all users
- `GET /api/User/get-user-profile` - Get current user profile
- `POST /api/User/create-user` - Create new user
- `PUT /api/User/update-user` - Update user
- `DELETE /api/User/delete-user/{id}` - Delete user
- `POST /api/User/upload-profile-img` - Upload profile image (FormData)

#### **Authentication**
- `POST /api/Auth/login` - User login
- `POST /api/Auth/signup` - User registration

#### **Pending APIs** (Services created, UI pending)
- Booking APIs (via `booking.service.ts`)
- Location APIs (via `location.service.ts`)
- Billing APIs (via `billing.service.ts`)

---

## ✅ Completed Features

### **1. User Management (Admin)** ✅
- View all users in table
- Add new user with form validation
- Edit existing user
- Delete user with confirmation
- Address field removed (as per requirements)

### **2. Authentication** ✅
- Login with JWT token
- Signup with validation
- Auto-redirect on token expiry
- Role-based access control

### **3. Profile Management** ✅
- View profile in modal
- Upload profile image
- Display profile image in navbar
- Real-time updates

### **4. Error Handling** ✅
- Global error interceptor
- User-friendly error messages
- Console logging for debugging
- HTTP request/response timing

### **5. Security** ✅
- JWT authentication
- HTTP interceptors
- Role-based guards
- Secure token storage

---

## 🚧 Pending Features

### **High Priority**
1. **Bookings Management** (Admin)
   - View/Add/Edit/Delete bookings
   - Booking status management

2. **Locations Management** (Admin)
   - View/Add/Edit/Delete parking locations
   - Location availability tracking

3. **Billings Management** (Admin)
   - View/Add/Edit/Delete billing records
   - Payment status tracking

4. **Driver Dashboard**
   - View personal bookings
   - View personal billings
   - Booking history

### **Enhancements**
- Replace `alert()` with toast notifications (ngx-toastr)
- Add confirmation modals for delete operations
- Implement refresh token mechanism
- Add image compression before upload
- Add drag-and-drop for file uploads
- Add pagination for large datasets
- Add search/filter functionality

---

## 🚀 Getting Started

### **Prerequisites**
- Node.js (v18+ recommended)
- npm (comes with Node.js)
- Angular CLI (`npm install -g @angular/cli`)

### **Installation Steps**

✅ **Dependencies are already installed** (`node_modules` exists)

If you need to reinstall:
```bash
cd d:\MyProjects\SmartPark\App\smart-park
npm install
```

### **Running the Application**

1. **Start Development Server**
```bash
npm start
# or
ng serve
```
Application will run at: `http://localhost:4200/`

2. **Build for Production**
```bash
npm run build
# or
ng build
```
Output: `dist/` directory

3. **Run Tests**
```bash
npm test
# or
ng test
```

---

## 🔧 Development Guidelines

### **Before Making Changes**
1. ✅ **DO NOT** modify existing functionality without understanding it
2. ✅ **DO** read the implementation summary files:
   - `COMPLETE_IMPLEMENTATION_SUMMARY.md`
   - `VIEW_EDIT_ADD_IMPLEMENTATION.md`
   - Other `*_SUMMARY.md` files

3. ✅ **DO** test existing features before adding new ones
4. ✅ **DO** follow the existing code structure and patterns

### **Code Standards**
- Use TypeScript strict mode
- Follow Angular style guide
- Use RxJS operators properly
- Implement error handling
- Add comments for complex logic
- Use meaningful variable names

### **Testing Checklist**
- [ ] Login/Logout functionality
- [ ] Admin dashboard access
- [ ] User CRUD operations
- [ ] Profile image upload
- [ ] Token expiry handling
- [ ] Error messages display
- [ ] Responsive design on mobile

---

## 📝 Important Notes

### **⚠️ Critical Information**

1. **Backend Dependency**
   - Application requires backend API at `https://localhost:7188/api/`
   - Ensure backend is running before starting frontend

2. **Token Storage**
   - JWT token stored in `localStorage` with key: `auth_token`
   - Token includes user role and ID

3. **Role-Based Access**
   - Admin: Full access to all management features
   - Driver: Limited to personal bookings and billings

4. **Existing Functionality**
   - User management is fully functional ✅
   - DO NOT modify without understanding the flow
   - Interceptors are critical for auth - DO NOT remove

5. **File Upload**
   - Profile images use FormData
   - Backend expects: `{ UserId, ImageFile }`

---

## 📚 Documentation Files

The project includes several documentation files:
- `COMPLETE_IMPLEMENTATION_SUMMARY.md` - Full feature implementation details
- `VIEW_EDIT_ADD_IMPLEMENTATION.md` - CRUD implementation guide
- `TOKEN_KEY_MISMATCH_FIX.md` - Token handling fixes
- `PROFILE_MODAL_FIX.md` - Profile modal implementation
- `INTERCEPTOR_FIX_COMPLETE.md` - Interceptor setup
- `ERROR_FIXES_AND_RUN.md` - Common error fixes

**Read these before making changes!**

---

## 🎯 Next Steps for Development

1. **Immediate Actions**
   - ✅ Run `npm start` to verify application works
   - ✅ Test login with valid credentials
   - ✅ Verify admin dashboard loads
   - ✅ Test user management features

2. **Development Workflow**
   - Create feature branch for new work
   - Implement one feature at a time
   - Test thoroughly before committing
   - Document changes in appropriate files

3. **Priority Features to Implement**
   - Bookings CRUD (follow Users pattern)
   - Locations CRUD (follow Users pattern)
   - Billings CRUD (follow Users pattern)
   - Driver dashboard functionality

---

## 🤝 Support & Maintenance

### **Common Issues**

**Issue**: Application won't start
- **Solution**: Run `npm install` and check for errors

**Issue**: API calls failing
- **Solution**: Verify backend is running at `https://localhost:7188/api/`

**Issue**: Token expired error
- **Solution**: This is expected behavior - login again

**Issue**: 404 on routes
- **Solution**: Check route guards and user role

---

## 📊 Project Status

**Overall Progress**: ~60% Complete

| Feature | Status | Notes |
|---------|--------|-------|
| Authentication | ✅ Complete | Login, Signup, JWT |
| User Management | ✅ Complete | Full CRUD |
| Profile Management | ✅ Complete | View, Upload Image |
| HTTP Interceptors | ✅ Complete | Auth, Error handling |
| Admin Dashboard | ✅ Complete | Layout, Navigation |
| Driver Dashboard | 🚧 Partial | Layout done, features pending |
| Bookings CRUD | 🚧 Pending | Service ready, UI needed |
| Locations CRUD | 🚧 Pending | Service ready, UI needed |
| Billings CRUD | 🚧 Pending | Service ready, UI needed |

---

**Last Updated**: October 23, 2025  
**Version**: 0.0.0  
**Maintainer**: Development Team

---

## 🎉 Ready to Start Development!

The project is properly set up with:
- ✅ Dependencies installed
- ✅ Core features working
- ✅ Clean architecture
- ✅ Documentation available

**You can now safely start working on new features without breaking existing functionality!**
