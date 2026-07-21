export async function compressImage(file, maxDimension = 1024, quality = 0.7) {
  const bitmap = await createImageBitmap(file);
  const scale  = Math.min(1, maxDimension / Math.max(bitmap.width, bitmap.height));

  const canvas  = document.createElement("canvas");
  canvas.width  = bitmap.width  * scale;
  canvas.height = bitmap.height * scale;
  canvas.getContext("2d").drawImage(bitmap, 0, 0, canvas.width, canvas.height);
  bitmap.close();

  return await new Promise(res => canvas.toBlob(res, "image/jpeg", quality));
}
