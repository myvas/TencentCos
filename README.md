# NOTES
This repo was abandonded! We recommand to use [Minio](https://min.io).

- [MinIO Client SDK for .NET](https://github.com/minio/minio-dotnet)
- [Systemd service for MinIO](https://github.com/minio/minio-service/tree/master/linux-systemd)
- [How to install minio storage server on debian 11](https://www.howtoforge.com/how-to-install-minio-storage-server-on-debian-11/)


# Myvas.AspNetCore.TencentCos 
[![NuGet (Pre-)Release](https://img.shields.io/nuget/vpre/Myvas.AspNetCore.TencentCos?label=nuget)](https://www.nuget.org/packages/Myvas.AspNetCore.TencentCos) 
[![GitHub (Pre-)Release Date](https://img.shields.io/github/release-date-pre/myvas/AspNetCore.TencentCos?label=github)](https://github.com/myvas/AspNetCore.TencentCos)

腾讯云对象存储SDK (cos-dotnet-sdk-v5)

* 对象存储(COS, Cloud Object Storage)
* 腾讯云(Qcloud/TencentYun)
* IDE: Visual Studio 2017 16.5.4+, dotnet-sdk-3.1.201+

## Demo Online
http://demo.cos.myvas.com

## Api Docs
https://cloud.tencent.com/document/product/436/10111

### Service
* GET Service/List Buckets: Impl, Demo

### Bucket
* HEAD Bucket: Impl, Tested
* PUT Bucket: Impl
* DELETE Bucket: Impl
* Get Bucket/List Objects: Impl, Demo

### Object
* HEAD Object: Impl, Tested
* PUT Object: Impl, Demo
* DELETE Object: Impl, Demo
* GET Object: Impl, Demo

## 注意
除遵守Linux和Windows中的文件命名规则外，文件名中也不能含有下列字符：
```
~#[]@!$&'()+,;=^
```

#### 其他版本实现
* 其他语言

[cos-xxx-sdk-v5](https://github.com/tencentyun?utf8=%E2%9C%93&q=cos+v5&type=&language=): Android, C, C++, iOS, Java, JavaScript, Node.js, PHP, Python

* 已过期版本

[cos-dotnet-sdk-v4](https://github.com/tencentyun/cos-donet-sdk-v4): Obsolete!


