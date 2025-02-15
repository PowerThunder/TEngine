﻿using System;
using System.IO;
using YooAsset;

namespace TEngine
{
    internal partial class ResourceManager
    {
        /// <summary>
        /// 资源文件解密服务类。
        /// </summary>
        private class GameDecryptionServices : IDecryptionServices
        {
            private const byte OffSet = 32;
            
            public ulong LoadFromFileOffset(DecryptFileInfo fileInfo)
            {
                return OffSet;
            }

            public byte[] LoadFromMemory(DecryptFileInfo fileInfo)
            {
                throw new NotImplementedException();
            }

            public Stream LoadFromStream(DecryptFileInfo fileInfo)
            {
                BundleStream bundleStream = new BundleStream(fileInfo.FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                return bundleStream;
            }

            public uint GetManagedReadBufferSize()
            {
                return 1024;
            }
        }
        
        /// <summary>
        /// 默认的分发资源查询服务类
        /// </summary>
        private class DefaultDeliveryQueryServices : IDeliveryQueryServices
        {
            public DeliveryFileInfo GetDeliveryFileInfo(string packageName, string fileName)
            {
                throw new NotImplementedException();
            }
            public bool QueryDeliveryFiles(string packageName, string fileName)
            {
                return false;
            }
        }
        
        /// <summary>
        /// 远程文件查询服务类。
        /// </summary>
        private class RemoteServices: IRemoteServices
        {
            private readonly string _defaultHostServer;
            private readonly string _fallbackHostServer;

            public RemoteServices()
            {
                _defaultHostServer = SettingsUtils.FrameworkGlobalSettings.HostServerURL;
                _fallbackHostServer = SettingsUtils.FrameworkGlobalSettings.FallbackHostServerURL;
            }
            
            public RemoteServices(string defaultHostServer, string fallbackHostServer)
            {
                _defaultHostServer = defaultHostServer;
                _fallbackHostServer = fallbackHostServer;
            }
            
            public string GetRemoteMainURL(string fileName)
            {
                return $"{_defaultHostServer}/{fileName}";
            }

            public string GetRemoteFallbackURL(string fileName)
            {
                return $"{_fallbackHostServer}/{fileName}";
            }
        }
    }

    public class BundleStream : FileStream
    {
        public const byte KEY = 128;

        public BundleStream(string path, FileMode mode, FileAccess access, FileShare share) : base(path, mode, access, share)
        {
        }

        public BundleStream(string path, FileMode mode) : base(path, mode)
        {
        }

        public override int Read(byte[] array, int offset, int count)
        {
            var index = base.Read(array, offset, count);
            for (int i = 0; i < array.Length; i++)
            {
                array[i] ^= KEY;
            }

            return index;
        }
    }
}