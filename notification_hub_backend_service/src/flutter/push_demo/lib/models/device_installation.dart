class DeviceInstallation {
  final String deviceId;
  final String platform;
  final String token;
  final List<String> tags;

  DeviceInstallation(this.deviceId, this.platform, this.token, this.tags);

  DeviceInstallation.fromJson(Map<String, dynamic> json)
      : deviceId = json['installationId'],
        platform = json['platform'],
        token = json['pushChannel'],
        tags = json['tags'];

  Map<String, dynamic> toJson() =>
    {
      'installationId': deviceId,
      'platform': platform,
      'pushChannel': token,
      'tags': tags,
    };
}