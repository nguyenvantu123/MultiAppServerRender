// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.


using BlazorIdentityApi.Helpers;

namespace BlazorIdentityApi.Resources
{
    public class IdentityServiceResources : IIdentityServiceResources
    {
        public virtual ResourceMessage UserUpdateFailed()
        {
            return new ResourceMessage()
            {
                Code = nameof(UserUpdateFailed),
                Description = "UserUpdateFailed"
            };
        }

        public virtual ResourceMessage UserRoleDeleteFailed()
        {
            return new ResourceMessage()
            {
                Code = nameof(UserRoleDeleteFailed),
                Description = "UserRoleDeleteFailed"
            };
        }

        public virtual ResourceMessage UserRoleCreateFailed()
        {
            return new ResourceMessage()
            {
                Code = nameof(UserRoleCreateFailed),
                Description = "UserRoleCreateFailed"
            };
        }
    
        public virtual ResourceMessage UserProviderDoesNotExist()
        {
            return new ResourceMessage()
            {
                Code = nameof(UserProviderDoesNotExist),
                Description = "UserProviderDoesNotExist"
            };
        }

        public virtual ResourceMessage UserProviderDeleteFailed()
        {
            return new ResourceMessage()
            {
                Code = nameof(UserProviderDeleteFailed),
                Description = "UserProviderDeleteFailed"
            };
        }

        public virtual ResourceMessage UserChangePasswordFailed()
        {
            return new ResourceMessage()
            {
                Code = nameof(UserChangePasswordFailed),
                Description = "UserChangePasswordFailed"
            };
        }

        public virtual ResourceMessage UserDoesNotExist()
        {
            return new ResourceMessage()
            {
                Code = nameof(UserDoesNotExist),
                Description = "UserChangePasswordFailed"
            };
        }

        public virtual ResourceMessage UserDeleteFailed()
        {
            return new ResourceMessage()
            {
                Code = nameof(UserDeleteFailed),
                Description = "UserDeleteFailed"
            };
        }

        public virtual ResourceMessage UserCreateFailed()
        {
            return new ResourceMessage()
            {
                Code = nameof(UserCreateFailed),
                Description = "UserCreateFailed"
            };
        }

        public virtual ResourceMessage UserClaimsDeleteFailed()
        {
            return new ResourceMessage()
            {
                Code = nameof(UserClaimsDeleteFailed),
                Description = "UserClaimsDeleteFailed"
            };
        }

        public virtual ResourceMessage UserClaimsCreateFailed()
        {
            return new ResourceMessage()
            {
                Code = nameof(UserClaimsCreateFailed),
                Description = "UserClaimsCreateFailed"
            };
        }

        public virtual ResourceMessage UserClaimsUpdateFailed()
        {
            return new ResourceMessage()
            {
                Code = nameof(UserClaimsCreateFailed),
                Description = "UserClaimsCreateFailed"
            };
        }

        public virtual ResourceMessage UserClaimDoesNotExist()
        {
            return new ResourceMessage()
            {
                Code = nameof(UserClaimDoesNotExist),
                Description = "UserClaimDoesNotExist"
            };
        }

        public virtual ResourceMessage RoleUpdateFailed()
        {
            return new ResourceMessage()
            {
                Code = nameof(RoleUpdateFailed),
                Description = "RoleUpdateFailed"
            };
        }

        public virtual ResourceMessage RoleDoesNotExist()
        {
            return new ResourceMessage()
            {
                Code = nameof(RoleDoesNotExist),
                Description = "RoleDoesNotExist"
            };
        }

        public virtual ResourceMessage RoleDeleteFailed()
        {
            return new ResourceMessage()
            {
                Code = nameof(RoleDeleteFailed),
                Description = "RoleDeleteFailed"
            };
        }

        public virtual ResourceMessage RoleCreateFailed()
        {
            return new ResourceMessage()
            {
                Code = nameof(RoleCreateFailed),
                Description = "RoleCreateFailed"
            };
        }

        public virtual ResourceMessage RoleClaimsDeleteFailed()
        {
            return new ResourceMessage()
            {
                Code = nameof(RoleClaimsDeleteFailed),
                Description = "RoleClaimsDeleteFailed"
            };
        }

        public virtual ResourceMessage RoleClaimsCreateFailed()
        {
            return new ResourceMessage()
            {
                Code = nameof(RoleClaimsCreateFailed),
                Description = "RoleClaimsCreateFailed"
            };
        }

        public virtual ResourceMessage RoleClaimsUpdateFailed()
        {
            return new ResourceMessage()
            {
                Code = nameof(RoleClaimsCreateFailed),
                Description = "RoleClaimsCreateFailed"
            };
        }

        public virtual ResourceMessage RoleClaimDoesNotExist()
        {
            return new ResourceMessage()
            {
                Code = nameof(RoleClaimDoesNotExist),
                Description = "RoleClaimDoesNotExist"
            };
        }

        public virtual ResourceMessage IdentityErrorKey()
        {
            return new ResourceMessage()
            {
                Code = nameof(IdentityErrorKey),
                Description = "IdentityErrorKey"
            };
        }
    }
}
